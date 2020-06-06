using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using DRimEditor.DetailView;
using System.Reflection;

namespace DRimEditor.DetailView
{
   

    public class DetailWrapper
    {
        public static Dictionary<Type, List<List<FieldInfo>>> typeToFieldDict = new Dictionary<Type, List<List<FieldInfo>>>();

        List<DetailSection> sections = new List<DetailSection>();

        public object parentObject;

        public static float _columnMargin = 8f;

        public bool align;
        public bool expanded = false;
        public bool initialized = false;

        public FieldDesc parentDesc;
        public CategoryDef cat;

        public DetailWrapper(object parent, CategoryDef category, bool align = true, FieldDesc parentDesc = null, bool directOnly = false)
        {
            parentObject = parent;
            this.parentDesc = parentDesc;
            this.align = align;
            cat = category;
        }

        public void Initialize()
        {
            List<List<FieldInfo>> fields;

            if (cat != null)
            {
                fields = cat.sectionFieldList;
            }
            else
            {
                Type parentType = parentObject.GetType();
                if (typeToFieldDict.ContainsKey(parentType))
                {
                    fields = typeToFieldDict[parentType];
                }
                else
                {
                    fields = DatabaseBuilder.GetSectionFieldList(parentType);

                    typeToFieldDict.Add(parentType, fields);
                }
            }

            foreach (var list in fields)
            {
                if (list.Count > 0)
                {
                    
                    sections.Add(new DetailSection(parentObject, list[0].DeclaringType.ToString(), list.ToArray(), this, align, parentDesc));
                }
            }
        }

        public void Draw(ref Vector3 cur, float width, DefView window = null)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            foreach(var section in sections)
            {
                section.Draw(ref cur, width, window);
            }
        }

        public string GetString()
        {
            string s = "";
            foreach(var section in sections)
            {
                s += section.GetString();
            }
            return s;
        }
    }
}
