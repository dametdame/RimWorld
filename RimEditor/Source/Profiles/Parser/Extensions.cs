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
using System.Linq.Expressions;

namespace DRimEditor
{
    public static class Extensions
    {

        public static string Push()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Push");
            return s.ToString();
        }

        public static string Push(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Push");
            return s.ToString();
        }

        public static string Pop()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Pop");
            return s.ToString();
        }

        public static string Pop(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Pop");
            return s.ToString();
        }

        public static string Enqueue()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Enqueue");
            return s.ToString();
        }

        public static string Enqueue(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Enqueue");
            return s.ToString();
        }

        public static string Dequeue()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Dequeue");
            return s.ToString();
        }

        public static string Dequeue(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Dequeue");
            return s.ToString();
        }

        public static string FindNull()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__FindNull");
            return s.ToString();
        }

        public static string FindNull(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__FindNull");
            return s.ToString();
        }

        public static string Find(Def def)
        {
            if (def == null)
                return FindNull();
            StringBuilder s = new StringBuilder();
            s.Append("__Find:");
            s.Append(def.GetType() + ":");
            s.Append(def.defName);
            return s.ToString();
        }

        public static string Find(object obj)
        {
            if (obj == null)
                return FindNull();
            if (typeof(Def).IsAssignableFrom(obj.GetType()))
                return Find(obj as Def);
            StringBuilder s = new StringBuilder();
            s.Append("__Find:");
            s.Append(obj.GetType() + ":");
            s.Append(obj.ToString());
            return s.ToString();
        }


        public static string Find(this string str, Def def)
        {
            if (def == null)
                return str.FindNull();
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Find:");
            s.Append(def.GetType() + ":");
            s.Append(def.defName);
            return s.ToString();
        }

        public static string Find(this string str, object obj)
        {
            if (obj == null)
                return str.FindNull();
            if (typeof(Def).IsAssignableFrom(obj.GetType()))
                return str.Find(obj as Def);
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Find:");
            s.Append(obj.GetType() + ":");
            s.Append(obj.ToString());
            return s.ToString();
        }


        public static string New(this string str, Type type)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__New:");
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                s.Append(type.AssemblyQualifiedName);
            }
            else
            {
                s.Append(type);
            }
            return s.ToString();
        }

        public static string StartParams(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("start params");
            return s.ToString();
        }

        public static string EndParams(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("end params");
            return s.ToString();
        }

        public static string Get(this string str, FieldInfo f)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Get:");
            s.Append(f.Name);
            return s.ToString();
        }

        public static string Get(this string str, Expression<Func<object, object>> f)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Get:");
            s.Append((f.Body as MemberExpression).Member.Name);
            return s.ToString();
        }

        public static string Get<T>(this string str, T obj) where T : class
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Get:");
            s.Append(typeof(T).GetProperties()[0].Name);
            return s.ToString();
        }     

        public static string GetStatic<T>(Type type, T obj) where T : class
        {
            StringBuilder s = new StringBuilder();
            s.Append("__GetStatic:");
            s.Append(type + ":");
            s.Append(typeof(T).GetProperties()[0].Name);
            return s.ToString();
        }

        public static string GetStatic(Type type, string field)
        {
            StringBuilder s = new StringBuilder();
            s.Append("__GetStatic:");
            s.Append(type + ":");
            s.Append(field);
            return s.ToString();
        }

        public static string GetStatic<T>(this string str, Type type, T obj) where T : class
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__GetStatic:");
            s.Append(type + ":");
            s.Append(typeof(T).GetProperties()[0].Name);
            return s.ToString();
        }

        public static string GetStatic(this string str, Type type, string field)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__GetStatic:");
            s.Append(type + ":");
            s.Append(field);
            return s.ToString();
        }

        public static string End(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__End");
            return s.ToString();
        }

        public static string And(this string str, FieldInfo field)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__And:");
            s.Append(field.Name);
            return s.ToString();
        }

        public static string From(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__From");
            return s.ToString();
        }

        public static string From(this string str, FieldInfo field)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__From:");
            s.Append(field.Name);
            return s.ToString();
        }

        public static string From<T>(this string str, T field) where T : class
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__From:");
            s.Append(typeof(T).GetProperties()[0].Name);
            return s.ToString();
        }

        public static string Set<T>(this string str, T obj) where T : class
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Set:");
            s.Append(typeof(T).GetProperties()[0].Name);
            return s.ToString();
        }

        public static string Set(this string str, FieldInfo f)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Set:");
            s.Append(f.Name);
            return s.ToString();
        }

        public static string Call(this string str, string method) 
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Call:");
            s.Append(method);
            return s.ToString();
        }

        public static string CallStatic(Type type, string method)
        {
            StringBuilder s = new StringBuilder();
            s.Append("__CallStatic:");
            s.Append(type + ":");
            s.Append(method);
            return s.ToString();
        }

        public static string CallStatic(this string str, Type type, string method)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__CallStatic:");
            s.Append(type + ":");
            s.Append(method);
            return s.ToString();
        }

        public static string Add()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Add");
            return s.ToString();
        }

        public static string Remove()
        {
            StringBuilder s = new StringBuilder();
            s.Append("__Remove");
            return s.ToString();
        }

        public static string Add(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Add");
            return s.ToString();
        }

        public static string Remove(this string str)
        {
            StringBuilder s = new StringBuilder();
            if (str != null && str != "")
                s.Append(str + ":");
            s.Append("__Remove");
            return s.ToString();
        }

    }
}


/*
 *   public enum CommandType
    {
        __None,
        __New,
        __Find,
        __Get,
        __GetStatic
        __From,
        __Set,
        __Call,
        __CallStatic,
        __Add,
        __Remove
    }
*/