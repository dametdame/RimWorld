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
    class DatabaseBuilder
    {

        public static bool initialized = false;

        public static List<FieldInfo> allDefFields = new List<FieldInfo>();

        //[Unsaved]

        static readonly string HelpPostFix = "_DetailCategoryDef",

        Humanoids = "Humanoids" + HelpPostFix,
        // recipes and research
        RecipeHelp = "Recipe" + HelpPostFix;


        public static object GetDefDatabaseList(Type objType)
        {
            Type defDatabase = typeof(DefDatabase<>).MakeGenericType(new Type[] { objType });
            MethodInfo AllDefsListForReading = AccessTools.PropertyGetter(defDatabase, "AllDefsListForReading");
            return AllDefsListForReading.Invoke(null, null);
        }

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
            foreach(var section in sections)
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

        static void ResolveDefs()
        {
            allDefFields.AddRange(GetTypeFields(typeof(Def)));

            Type[] allDefSubclasses = AllTypeSubclasses(typeof(Def));
            
            foreach (Type subclassType in allDefSubclasses)
            {
                if (subclassType == typeof(ItemDetailDef))
                    continue;
                ResolveSubDef(subclassType);
            }
        }

        static void ResolveSubDef(Type subclassType)
        {
            var defs = new List<Def>();
            ICollection allDefs = GetDefDatabaseList(subclassType) as ICollection;
            IEnumerator e = allDefs.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.GetType() != subclassType)
                {
                    continue;
                }
                defs.AddUnique(e.Current as Def);
            }
            if (defs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey(subclassType.Name.CapitalizeFirst() + HelpPostFix, subclassType.Name.CapitalizeFirst(), ResourceBank.String.AutoHelpCategoryDefs, subclassType);
            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(defs, helpCategoryDef);
        }

        public static void ResolveImpliedDefs()
        {
            ResolveDefs(); 
            ResolveReferences();
            initialized = true;
            MainWindow.defExplorerWindow.PreOpen();
            Find.WindowStack.Add(MainWindow.instance);   
        }

        static void ResolveReferences()
        {
            foreach (var helpCategory in DefDatabase<CategoryDef>.AllDefsListForReading)
            {
                helpCategory.Recache();
            }
            DefExplorerWindow.Recache();
        }    

        static void ResolveDefList<T>(IEnumerable<T> defs, CategoryDef category) where T : Def
        {
            // Get help database
            HashSet<Def> processedDefs =
                new HashSet<Def>(DefDatabase<ItemDetailDef>.AllDefsListForReading.Select(h => h.keyDef));

            // Scan through defs and auto-generate help
            foreach (T def in defs)
            {                
                // Check if the def doesn't already have a help entry
                if (!processedDefs.Contains(def))
                {
                    // Make a new one
                    ItemDetailDef helpDef = null;
                    try
                    {
                        helpDef = HelpForDef(def, category);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("HelpTab :: Failed to build help for: " + def + "\n\t" + e);
                    }

                    // Inject the def
                    if (helpDef != null)
                    {
                        DefDatabase<ItemDetailDef>.Add(helpDef);
                    }
                }
            }
        }

        static CategoryDef HelpCategoryForKey(string key, string label, string modname, Type subclassType)
        {
            // Get help category
            var helpCategoryDef = DefDatabase<CategoryDef>.GetNamed(key, false);

            if (helpCategoryDef == null)
            {
                // Create new designation help category
                helpCategoryDef = new CategoryDef(subclassType);
                helpCategoryDef.defName = key;
                helpCategoryDef.keyDef = key;
                helpCategoryDef.label = label;
                helpCategoryDef.ModName = modname;

                DefDatabase<CategoryDef>.Add(helpCategoryDef);
            }

            return helpCategoryDef;
        }

        public static ItemDetailDef HelpForDef<T>(T def, CategoryDef category) where T : Def
        {
            var itemDef = new ItemDetailDef();
            itemDef.defName = def.defName + "_"+ category.defName + "_Detail";
            itemDef.keyDef = def;
            itemDef.label = def.label;
            itemDef.category = category;
            itemDef.description = def.description;

            DetailWrapper subDefSection = new DetailWrapper(def, category, true, null);
            itemDef.HelpDetailWrappers.Add(subDefSection);
            return itemDef;
        }
    }
}
