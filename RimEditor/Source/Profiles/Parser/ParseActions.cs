using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Reflection;
using System.IO;
using HarmonyLib;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DRimEditor
{
    class ParseActions
    {

        public static Pair<FieldInfo, object> GetCompareValue(IEnumerator<string> items, Type childType)
        {
            FieldInfo fieldInfo = AccessTools.Field(childType, items.Current);
            Parser.notFinished = items.MoveNext();
            object targetObj = Parser.EvalNext(items, null);
            return new Pair<FieldInfo, object>(fieldInfo, targetObj);
        }

        public static object DoFromDirect(IEnumerator<string> items, ICollection collection)
        {
            if (Parser.ParseCommand(items.Current) != CommandType.__Find)
            {
                throw new Exception("FromDirect without Find command");
            }
            object targetObj = Parser.EvalNext(items, null);
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object cur = enumerator.Current;
                if (cur.Equals(targetObj))
                    return cur;
            }
            throw new Exception("Failed to find object in DoFromDirect");
        }

        public static object DoFrom(IEnumerator<string> items, object obj)
        {
            if (obj == null)
            {
                throw new Exception("RimEditor: From command with null object");
            }
            ICollection collection = obj as ICollection;
            if (collection == null)
            {
                throw new Exception("RimEditor: From command with object not castable to ICollection");
            }
            Type childType = obj.GetType().GenericTypeArguments[0];

            if (ParseHelper.CanMakeTypedObject(childType)) // directly findable & we should never be comparing fields
            {
                return DoFromDirect(items, collection);
            }
            
            List<Pair<FieldInfo, object>> comparers = new List<Pair<FieldInfo, object>>();
            comparers.Add(GetCompareValue(items, childType));

            while (Parser.ParseCommand(items.Current) == CommandType.__And)
            {
                Parser.notFinished = items.MoveNext();
                comparers.Add(GetCompareValue(items, childType));
            }
            if (Parser.ParseCommand(items.Current) != CommandType.__End)
            {
                throw new Exception("RimEditor: From without matching End, maybe no Ands");
            }
            Parser.notFinished = items.MoveNext(); // now 1 past __End

            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object candidate = enumerator.Current;

                bool found = true;
                foreach (var comparer in comparers)
                {
                    object candidateFieldValue = comparer.First.GetValue(candidate);
                    if (candidateFieldValue == null)
                    {
                        if (comparer.Second != null)
                        {
                            found = false;
                            break;
                        }
                    }
                    else if (!candidateFieldValue.Equals(comparer.Second))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return candidate;
                }

            }
            throw new Exception("RimEditor: From Command, could not find item matching criteria: " + comparers.ToStringSafeEnumerable() 
                + "\nCollection was " + collection.ToStringSafeEnumerable());
        }

        public static object DoGetStatic(IEnumerator<string> items)
        {
            Type type = ParseHelper.GetTypeOf(items.Current);
            items.MoveNext();
            FieldInfo fieldInfo = AccessTools.Field(type, items.Current);
            Parser.notFinished = items.MoveNext();
            return fieldInfo.GetValue(null);
        }

        public static object DoGet(IEnumerator<string> items, object obj)
        {
            if (obj == null)
            {
                throw new Exception("RimEditor: Get command with null object");
            }
            string value = items.Current;
            FieldInfo fieldInfo = AccessTools.Field(obj.GetType(), value);
            Parser.notFinished = items.MoveNext();
            return fieldInfo.GetValue(obj);
        }

        public static object DoSet(IEnumerator<string> items, object obj)
        {
            string value = items.Current;
            FieldInfo fieldInfo = AccessTools.Field(obj.GetType(), value);
            Parser.notFinished = items.MoveNext();
            object targetObj = Parser.Parse(items, obj);
            object oldObj = fieldInfo.GetValue(obj);
            fieldInfo.SetValue(obj, targetObj);
            //if (Parser.notFinished)
            //    Parser.notFinished = items.MoveNext();
            return obj;
        }

        public static object CompleteCall(IEnumerator<string> items, Type type, object obj)
        {
            string value = items.Current;
            MethodInfo methodInfo = AccessTools.Method(type, value);
            Parser.notFinished = items.MoveNext();

            List<object> parameters = null;
            if (Parser.notFinished && items.Current == "start params")
            {
                parameters = GetParams(items);
            }
            return methodInfo.Invoke(obj, parameters?.ToArray());
        }

        public static object DoCall(IEnumerator<string> items, object obj)
        {
            if (obj == null)
            {
                throw new Exception("RimEditor: Call command with null object");
            }
            return CompleteCall(items, obj.GetType(), obj);
        }

        public static object DoStaticCall(IEnumerator<string> items)
        {
            Type type = ParseHelper.GetTypeOf(items.Current);
            items.MoveNext();
            return CompleteCall(items, type, null);
        }

        public static object CompleteFind(IEnumerator<string> items, object obj)
        {
            if (!Parser.notFinished)
                return obj;
            object curObj = obj;
            CommandType command;
            while (Parser.notFinished)
            {
                try
                {
                    command = (CommandType)Enum.Parse(typeof(CommandType), items.Current);
                }
                catch
                {
                    break;
                }
                if (command == CommandType.__Get || command == CommandType.__From || command == CommandType.__Call || command == CommandType.__CallStatic)
                {
                    Parser.notFinished = items.MoveNext();
                    curObj = Parser.Apply(items, command, curObj);
                }
                else
                {
                    break;
                }
            }
            return curObj;
        }

        public static object FastForward(IEnumerator<string> items, object obj)
        {
            if (!Parser.notFinished)
                return obj;
            CommandType command;
            while (Parser.notFinished)
            {
                try
                {
                    command = (CommandType)Enum.Parse(typeof(CommandType), items.Current);
                }
                catch
                {
                    Parser.notFinished = items.MoveNext();
                    continue;
                }
                if (command == CommandType.__Get || command == CommandType.__From || command == CommandType.__Call || command == CommandType.__CallStatic)
                {
                    Parser.notFinished = items.MoveNext();
                }
                else
                {
                    break;
                }
            }
            return obj;
        }

        public static object DoFind(IEnumerator<string> items)
        {
            Type type = ParseHelper.GetTypeOf(items.Current);
            items.MoveNext();
            string value = items.Current;
            object curObj;
            curObj = ParseHelper.GetTypedObject(type, value);
            Parser.notFinished = items.MoveNext();
            return CompleteFind(items, curObj);
        }

        public static object DoFindNull()
        {
            return null;
        }

        public static List<object> GetParams(IEnumerator<string> items) // place me 1 after end params and set finished
        {
            if (items.Current == "start params")
            {
                List<object> parameters = new List<object>();
                items.MoveNext();
                while (items.Current != "end params")
                {
                    CommandType command = (CommandType)Enum.Parse(typeof(CommandType), items.Current);
                    if (command != CommandType.__Find && command != CommandType.__New && command != CommandType.__CallStatic)
                    {
                        throw new Exception("GetParams: Tried to get param without __Find, __New, or __CallStatic");
                    }
                    items.MoveNext();
                    parameters.Add(Parser.Apply(items, command, null));
                    if (!Parser.notFinished)
                    {
                        throw new Exception("GetParams: Reached end of params without end params");
                    }
                }
                Parser.notFinished = items.MoveNext();
                return parameters;
            }
            else
            {
                throw new Exception("GetParams with no start params");
            }
        }

        public static object DoNew(IEnumerator<string> items)
        {
            Type type = ParseHelper.GetTypeOf(items.Current);
            Parser.notFinished = items.MoveNext();
            List<object> parameters = null;
            if (Parser.notFinished && items.Current == "start params")
            {
                parameters = GetParams(items);
            }
            return ParseHelper.MakeTypedObjectWithParams(type, parameters);
        }

        public static object DoAdd(IEnumerator<string> items, object obj)
        {
            object targetObj;
            if (obj == null) // assume we're adding to def database
            {
                targetObj = Parser.Parse(items);
                ParseHelper.AddToDefDatabase(targetObj);
                return targetObj;
            }

            Type collectionType = obj.GetType();
            MethodInfo add = AccessTools.Method(collectionType, "Add");

            targetObj = Parser.Parse(items, obj); // should be at end of command now

            add.Invoke(obj, new object[] { targetObj });
            return targetObj;
        }

        public static object DoRemove(IEnumerator<string> items, object obj)
        {
            object targetObj;
            if (obj == null) // assume we're removing from def database
            {
                targetObj = Parser.Parse(items);
                ParseHelper.RemoveFromDefDatabase(targetObj);
                return targetObj;
            }

            Type collectionType = obj.GetType();
            MethodInfo remove = AccessTools.Method(collectionType, "Remove");
            targetObj = Parser.Parse(items, obj); // should be at end of command now
            remove.Invoke(obj, new object[] { targetObj });
            return targetObj;
        }

        public static object DoEnqueue(IEnumerator<string> items, object obj)
        {
            object targetObj = Parser.Parse(items, obj);
            Parser.queue.Enqueue(targetObj);
            return targetObj;
        }
        public static object DoDequeue()
        {
            return Parser.queue.Dequeue();
        }
        public static object DoPush(IEnumerator<string> items, object obj)
        {
            object targetObj = Parser.Parse(items, obj);
            Parser.stack.Push(targetObj);
            return targetObj;
        }
        public static object DoPop()
        {
            return Parser.stack.Pop();
        }

    }
}
