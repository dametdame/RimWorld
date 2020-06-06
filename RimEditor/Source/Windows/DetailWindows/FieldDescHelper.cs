using DRimEditor.Research;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Noise;
using static DRimEditor.Extensions;

namespace DRimEditor.DetailView
{
    public static class FieldDescHelper
    {
        public static string GetValueString(object obj)
        {
            if (obj != null && obj.ToString() != null)
            {
                return obj.ToString();
            }
            return "null";
        }
        public static bool IsEnumerable(Type type)
        {
            return !IsDirectEditable(type) && (typeof(IEnumerable).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type));
        }

        public static bool IsDirectEditable(Type type)
        {
            return (typeof(Def).IsAssignableFrom(type) || type == typeof(bool) || type.IsEnum || IsConvertibleFromText(type));
        }

        public static bool IsConvertibleFromText(Type type)
        {
            return ((type.IsPrimitive && type != typeof(bool)) || type == typeof(string) || type == typeof(String) || type == typeof(Decimal)); //|| (type.GetConstructor(new Type[]{typeof(string)}) != null));
        }

        public static bool CanBeNull(Type type)
        {
            return type == typeof(string) || !IsConvertibleFromText(type);
        }

        public static bool CanMakeNew(Type type)
        {
            if (type == null)
                return false;
            return 
                !typeof(Type).IsAssignableFrom(type) && 
                (IsDirectEditable(type) 
                || (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null
                    && !type.IsAbstract));
        }

        public static float CalcWidth(string[] lines, bool directEditable)
        {
            //if (directEditable)
                return lines.Select(s => Text.CalcSize(s).x).Max() + 35f;
            //return lines.Select(s => Text.CalcSize(s).x).Max() + 15f;
        }

        public static float CalcHeight(string[] lines, float lineHeight)
        {
            float valueHeight = (lineHeight * lines.Count()) + 10f;
            return valueHeight;
        }

        public static string GetAddDropdownPayload(Type type)
        {
            return "Add";
        }

        public static string GetDropdownPayload(Type type)
        {
            if (type != null && !typeof(Def).IsAssignableFrom(type) && !type.IsEnum)
                return "null";
            return type.Name;
        }

        public static List<Pair<FieldInfo, object>> GetAllDirectEditableFieldValues(object obj)
        {
            Type objType = obj.GetType();
            FieldInfo[] fields = GetAllDirectEditableFields(objType);
            List<Pair<FieldInfo, object>> pairs = new List<Pair<FieldInfo, object>>();
            foreach (var field in fields)
            {
                pairs.Add(new Pair<FieldInfo, object>(field, field.GetValue(obj)));
            }
            return pairs;
        }

        public static FieldInfo [] GetAllFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            Type curType = type;
            while (curType != null && curType != typeof(object))
            {
                FieldInfo[] newFields = curType.GetFields();
                if (newFields != null)
                    fields.AddRangeUnique(newFields);
                curType = curType.BaseType;
            }
            return fields.ToArray(); // no enumerables in enumerables right now
        }

        public static FieldInfo[] GetAllDirectEditableFields(Type type)
        {
            return GetAllFields(type).Where(f => IsDirectEditable(f.FieldType)).ToArray();
        }

        public static IEnumerable<Widgets.DropdownMenuElement<string>> MakeDropdownMenu(Type type, Func<object, bool> func)
        {
            List<Widgets.DropdownMenuElement<string>> options = new List<Widgets.DropdownMenuElement<string>>(); ;
            if (type == typeof(bool))
            {
                options.Add(new Widgets.DropdownMenuElement<string>
                {
                    option = new FloatMenuOption("True", () => func(true), MenuOptionPriority.Default),
                    payload = "true"
                });
                options.Add(new Widgets.DropdownMenuElement<string>
                {
                    option = new FloatMenuOption("False", () => func(false), MenuOptionPriority.Default),
                    payload = "false"
                });
                return options;
            }
            else if (type.IsEnum)
            {
                var vals = Enum.GetValues(type);
                foreach (var val in vals)
                {
                    string name = Enum.GetName(type, val);
                    options.Add(new Widgets.DropdownMenuElement<string>
                    {
                        option = new FloatMenuOption(name, () => func(val), MenuOptionPriority.Default),
                        payload = name
                    });
                }
                return options;
            }
            else if (!typeof(Def).IsAssignableFrom(type)) // not def, enum, or bool
            {
                return new Widgets.DropdownMenuElement<string>[]
                {
                    new Widgets.DropdownMenuElement<string>
                    {
                        option = new FloatMenuOption("Nothing found", null, MenuOptionPriority.Default),
                        payload = null
                    }
                };
            }
            IEnumerator iterator;
            if (type == typeof(Def)) 
            {
                List<Def> allDefs = new List<Def>();
                Type[] defTypes = DatabaseBuilder.AllTypeSubclasses(typeof(Def));
                foreach (Type t in defTypes)
                {
                    allDefs.AddRange(DatabaseBuilder.GetDefDatabaseList(t) as ICollection<Def>);
                }
                iterator = allDefs.GetEnumerator();
            }
            else
                iterator = (DatabaseBuilder.GetDefDatabaseList(type) as ICollection).GetEnumerator();

            while (iterator.MoveNext())
            {
                object cur = iterator.Current;
                string name = (cur as Def).defName;
                options.Add(new Widgets.DropdownMenuElement<string>
                {
                    option = new FloatMenuOption(name, () => func(cur), MenuOptionPriority.Default),
                    payload = name
                });
            }
            options.SortBy(x => x.payload);
            options.Insert(0, new Widgets.DropdownMenuElement<string>
            {
                option = new FloatMenuOption("null", () => func(null), MenuOptionPriority.High),
                payload = "null"
            });
            return options;
        }

        public static void GenerateFromCommand(object obj, ref string str)
        {
            if (str != null && str != "")
                str += ":";

            List<Pair<FieldInfo, object>> pairs = GetAllDirectEditableFieldValues(obj);
            str += "".From(pairs[0].First);
            str += ":" + Find(pairs[0].Second);
            int count = pairs.Count();
            for (int i = 1; i < count; i++)
            {
                str += ":" + "".And(pairs[i].First).Find(pairs[i].Second);
            }
            str += ":" + "".End();
        }


    }
}
