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

namespace DRimEditor
{
    public static class ParseHelper
    {

        private static readonly Regex sWhitespace = new Regex(@"\s+");

        public static string rimWorldVersion
        {
            get
            {
                if (cachedRimworldVersion == null)
                    cachedRimworldVersion = GetRimWorldVersion();
                return cachedRimworldVersion;
            }
        }
        private static string cachedRimworldVersion = null;

        public static string coreLibVersion
        {
            get
            {
                if (cachedCoreLibVersion == null)
                    cachedCoreLibVersion = GetCorelibVersion();
                return cachedCoreLibVersion;
            }
        }
        private static string cachedCoreLibVersion = null;

        private static string GetRimWorldVersion()
        {
            string readout = typeof(Verse.ThingDef).AssemblyQualifiedName;
            List<string> parts = new List<string>(readout.Split(','));
            for (int i = 0; i < parts.Count; i++)
            {
                string cur = parts[i];
                sWhitespace.Replace(cur, "");
                if (cur == "Assembly-CSharp")
                {
                    return parts[i + 1];
                }
            }
            Log.Error("Could not find current rimworld version");
            return null;
        }

        private static string GetCorelibVersion()
        {
            string readout = typeof(System.Array).AssemblyQualifiedName;
            List<string> parts = new List<string>(readout.Split(','));
            for (int i = 0; i < parts.Count; i++)
            {
                string cur = parts[i];
                sWhitespace.Replace(cur, "");
                if (cur == "mscorlib")
                {
                    return parts[i + 1];
                }
            }
            Log.Error("Could not find current corelib version");
            return null;
        }

        public static string FixVersions(string s)
        {
            List<string> parts = new List<string>(s.Split(','));
            for (int i = 0; i < parts.Count; i++)
            {
                string cur = parts[i];
                sWhitespace.Replace(cur, "");
                if (cur == "mscorlib")
                {
                    parts[i + 1] = rimWorldVersion;
                    i++;
                }
                else if (cur == "Assembly-CSharp")
                {
                    parts[i + 1] = coreLibVersion;
                    i++;
                }
            }
            return string.Join(",", parts);
        }


        public static object GetDefDatabaseValue(Type objType, string val)
        {
            Type defDatabase = typeof(DefDatabase<>).MakeGenericType(new Type[] { objType });
            MethodInfo getNamed = AccessTools.Method(defDatabase, "GetNamed");
            return getNamed.Invoke(null, new object[] { val, true });
        }

        public static void AddToDefDatabase(object obj)
        {
            Type objType = obj.GetType();
            Type defDatabase = typeof(DefDatabase<>).MakeGenericType(new Type[] { objType });
            MethodInfo add = AccessTools.Method(defDatabase, "Add", new Type[]{ objType });
            add.Invoke(null, new object[] { obj });
        }

        public static void RemoveFromDefDatabase(object obj)
        {
            Type objType = obj.GetType();
            Type defDatabase = typeof(DefDatabase<>).MakeGenericType(new Type[] { objType });
            MethodInfo add = AccessTools.Method(defDatabase, "Remove", new Type[] { objType });
            add.Invoke(null, new object[] { obj });
        }

        public static Type GetGenericType(Type collectionType, Type objType)
        {
            return collectionType.MakeGenericType(new Type[] { objType });
        }

        public static object MakeTypedObjectWithParams(Type objType, List<object> paramList)
        {
            if (paramList == null)
            {
                return MakeTypedObject(objType);
            }
            return MakeTypedObject(objType, paramList);
        }

        public static object MakeTypedObject(Type objType, List<object> paramObjects = null)
        {
            if (paramObjects != null)
            {
                return Activator.CreateInstance(objType, paramObjects.ToArray(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }
            return Activator.CreateInstance(objType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }

        public static object GetTypedObject(Type objType, string val)
        {
            if(val == "" || val == null)
            {
                return null;
            }
            else if (typeof(Def).IsAssignableFrom(objType))
            {
                return ParseHelper.GetDefDatabaseValue(objType, val);
            }
            else if (objType.IsEnum)
            {
                return Enum.Parse(objType, val);
            }
            else if (objType.IsPrimitive || objType == typeof(bool) || objType == typeof(string) || objType == typeof(String) || objType == typeof(Decimal)) // primitives & primitivelike
            {
                return Convert.ChangeType(val, objType);
            }
            return null; // no way to find an existing object of this type
        }

        public static bool CanMakeTypedObject(Type objType)
        {
            return (typeof(Def).IsAssignableFrom(objType) || objType.IsEnum || objType.IsPrimitive || objType == typeof(bool) || objType == typeof(string) || objType == typeof(String) || objType == typeof(Decimal));
        }

        public static Type GetTypeOf(string s)
        {
            return AccessTools.TypeByName(ParseHelper.FixVersions(s));
        }
    }
}
