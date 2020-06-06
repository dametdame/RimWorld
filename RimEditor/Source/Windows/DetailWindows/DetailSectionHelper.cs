﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
	// Token: 0x0200000D RID: 13
	public class DetailSectionHelper
	{

        #region DefStringTriplet list builder

        public static List<DefStringTriplet> BuildDefStringTripletList(List<Def> defs, string[] prefixes = null, string[] suffixes = null)
        {
            bool hasPrefix = false, hasSuffix = false;
            if (prefixes != null)
            {
                if (prefixes.Length != defs.Count)
                {
                    throw new Exception("Prefix array length does not match Def list length.");
                }
                hasPrefix = true;
            }

            if (suffixes != null)
            {
                if (suffixes.Length != defs.Count)
                {
                    throw new Exception("Suffix array length does not match Def list length.");
                }
                hasSuffix = true;
            }

            // prepare list of unique indices, filter out duplicates.
            List<Def> seen = new List<Def>();
            List<int> unique = new List<int>();

            for (int i = 0; i < defs.Count; i++)
            {
                if (seen.Count(def => def == defs[i]) == 0)
                {
                    unique.Add(i);
                    seen.Add(defs[i]);
                }
            }

            List<DefStringTriplet> ret = new List<DefStringTriplet>();
            foreach (int i in unique)
            {
                ret.Add(new DefStringTriplet(defs[i], hasPrefix ? prefixes[i] : null, hasSuffix ? suffixes[i] : null));
            }

            return ret;
        }

        #endregion

        #region GUI drawer functions

        public static void DrawText(ref Vector3 cur, float width, string text)
        {
            float height = Text.CalcHeight(text, width);
            Rect rect = new Rect(cur.x, cur.y, width, height);
            Widgets.Label(rect, text);
            cur.y += height - 6f; // offset to make lineheights fit better
        }

        public static void DrawLink(ref Vector3 cur, Rect container, Def def)
        {

        }

        #endregion

        public static List<StringDescTriplet> BuildStringDescTripletList(string[] stringDescs, string[] prefixes, string[] suffixes)
        {
            bool hasPrefix = false, hasSuffix = false;
            if (prefixes != null)
            {
                if (prefixes.Length != stringDescs.Length)
                {
                    throw new Exception("Prefix array length does not match stringDescs length.");
                }
                hasPrefix = true;
            }

            if (suffixes != null)
            {
                if (suffixes.Length != stringDescs.Length)
                {
                    throw new Exception("Suffix array length does not match stringDescs length.");
                }
                hasSuffix = true;
            }

            List<StringDescTriplet> ret = new List<StringDescTriplet>();
            for (int i = 0; i < stringDescs.Length; i++)
            {
                ret.Add(new StringDescTriplet(stringDescs[i], hasPrefix ? prefixes[i] : null, hasSuffix ? suffixes[i] : null));
            }

            return ret;
        }

        public static List<FieldDesc> BuildFieldDescList(object obj, FieldInfo [] fields, FieldDesc parentDesc)
		{
            List<FieldDesc> ret = new List<FieldDesc>();
            foreach (FieldInfo field in fields)
            {
                ret.Add(new FieldDesc(obj, field, parentDesc));
            }
            return ret;
		}
	}
}
