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

    

    public static class Parser
    {
        public static bool notFinished = true;
        public static Queue<object> queue = new Queue<object>();
        public static Stack<object> stack = new Stack<object>();

        public static object StartParse(string line)
        {
            IEnumerable<string> items = line.Split(':');
            IEnumerator<string> enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            return Parse(enumerator);
        }

        public static object Parse(IEnumerator<string> items, object obj = null)
        {
            object currentObj = obj;
            while (notFinished)
            {
                currentObj = EvalNext(items, currentObj);
            }
            return currentObj;
        }

        public static object PassThrough(IEnumerator<string> items, object obj = null)
        {
            notFinished = false;
            return obj;
        }

        public static CommandType ParseCommand(string item)
        {
            return (CommandType)Enum.Parse(typeof(CommandType), item);
        }

        public static object EvalNext(IEnumerator<string> items, object obj)
        {
            CommandType command = ParseCommand(items.Current);
            notFinished = items.MoveNext();
            return Apply(items, command, obj);
        }

        public static object Apply(IEnumerator<string> items, CommandType command, object obj)
        {
            try
            {
                if (command == CommandType.__Find)
                {
                    return ParseActions.DoFind(items); 
                }
                else if (command == CommandType.__FindNull)
                {
                    return ParseActions.DoFindNull();
                }
                else if (command == CommandType.__New)
                {
                    return ParseActions.DoNew(items);
                }
                else if (command == CommandType.__Get)
                {
                    return ParseActions.DoGet(items, obj); 
                }
                else if (command == CommandType.__GetStatic)
                {
                    return ParseActions.DoGetStatic(items);
                }
                else if (command == CommandType.__From)
                {
                    return ParseActions.DoFrom(items, obj); 
                }
                else if (command == CommandType.__Add)
                {
                    return ParseActions.DoAdd(items, obj);
                }
                else if (command == CommandType.__Set)
                {
                    return ParseActions.DoSet(items, obj); // end
                }
                else if (command == CommandType.__Remove)
                {
                    return ParseActions.DoRemove(items, obj);
                }
                else if (command == CommandType.__Call)
                {
                    return ParseActions.DoCall(items, obj);
                }
                else if (command == CommandType.__CallStatic)
                {
                    return ParseActions.DoStaticCall(items);
                }
                else if (command == CommandType.__Enqueue)
                {
                    return ParseActions.DoEnqueue(items, obj);
                }
                else if (command == CommandType.__Dequeue)
                {
                    return ParseActions.DoDequeue();
                }
                else if (command == CommandType.__Push)
                {
                    return ParseActions.DoPush(items, obj);
                }
                else if (command == CommandType.__Pop)
                {
                    return ParseActions.DoPop();
                }
                else
                {
                    throw new Exception("Unknown command type " + command.ToString());
                    
                }
            }
            catch (Exception e)
            {
                Log.Error("Error applying command " + command.ToString() + ", message: " + e.Message);
                throw;
            }
        }

        public static bool ReturnFalse()
        {
            return false;
        }
    }

    public enum CommandType
    {
        __None,
        __Enqueue,
        __Dequeue,
        __Push,
        __Pop,
        __New,
        __Find,
        __FindNull,
        __Get,
        __GetStatic,
        __From,
        __And,
        __End,
        __Set,
        __Call,
        __CallStatic,
        __Add,
        __Remove
    }
}
