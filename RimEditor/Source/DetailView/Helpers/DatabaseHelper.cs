using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DRimEditor.Windows;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DRimEditor.DetailView
{
    public static class DatabaseHelper
    {
        public static object GetGenericList(Type objType)
        {
            Type list = typeof(List<>).MakeGenericType(new Type[] { objType });
            return Activator.CreateInstance(list, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }
        public static FieldInfo[] GetTypeFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => !f.IsLiteral).ToArray();
        }

        public static List<List<FieldInfo>> GetSectionFieldList(Type type)
        {
            List<List<FieldInfo>> sections = new List<List<FieldInfo>>();
            while (type != null && type != typeof(object))
            {
                var newFields = GetTypeFields(type);
                for (int i = 0; i < sections.Count; i++)
                {
                    sections[i] = sections[i].Where(x => !newFields.Any(y => y.FieldType == x.FieldType && y.Name == x.Name)).ToList();
                }
                sections.Insert(0, newFields.ToList());
                type = type.BaseType;
            }
            foreach (var section in sections)
            {
                if (section.Count > 0)
                    section.SortBy(f => f.Name);
            }
            return sections;
        }

        public static Type[] AllTypeSubclasses(Type type)
        {
            return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetTypes()
                    where type.IsAssignableFrom(assemblyType)
                    select assemblyType).ToArray();
        }


    }
}
