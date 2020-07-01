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
    class DefDatabaseBuilder
    {

        public static bool initialized = false;

        public static List<FieldInfo> allDefFields = new List<FieldInfo>();

        public static object GetDefDatabaseList(Type objType)
        {
            Type defDatabase = typeof(DefDatabase<>).MakeGenericType(new Type[] { objType });
            MethodInfo AllDefsListForReading = AccessTools.PropertyGetter(defDatabase, "AllDefsListForReading");
            return AllDefsListForReading.Invoke(null, null);
        }

        static void ResolveDefs()
        {
            allDefFields.AddRange(DatabaseHelper.GetTypeFields(typeof(Def)));

            Type[] allDefSubclasses = DatabaseHelper.AllTypeSubclasses(typeof(Def));
            
            foreach (Type subclassType in allDefSubclasses)
            {
                if (subclassType == typeof(ItemDetail) || subclassType.Name == "HelpDef")
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
            var helpCategoryDef = DetailCategoryForType(subclassType, subclassType.Name.CapitalizeFirst(), ResourceBank.String.RimEditorDefs);
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
            foreach (DetailCategory helpCategory in DefExplorerWindow.detailCategories.Values)
            {
                helpCategory.Recache();
            }
            DefExplorerWindow.Recache();
        }    

        static void ResolveDefList<T>(IEnumerable<T> defs, DetailCategory category) where T : Def
        {
            HashSet<Def> processedDefs = new HashSet<Def>(DefExplorerWindow.itemDetails.Select(h => h.Value.keyObject as Def));

            foreach (T def in defs)
            {                
                if (!processedDefs.Contains(def))
                {
                    ItemDetail item = null;
                    try
                    {
                        item = MakeItemDetail(def, category);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("RimEditor: failed to create detail for " + def + "\n\t" + e);
                    }

                    if (item != null)
                    {
                        DefExplorerWindow.itemDetails.Add(def, item);
                    }
                }
            }
        }

        static DetailCategory DetailCategoryForType(Type subclassType, string label, string modname)
        {
            // Get help category
            //var helpCategoryDef = DefDatabase<DetailCategory>.GetNamed(key, false);
            DetailCategory detailCategory = DefExplorerWindow.detailCategories.TryGetValue(subclassType);

            if (detailCategory == null)
            {
                // Create new designation help category
                detailCategory = new DetailCategory(subclassType);
                //detailCategory.keyDef = label;
                detailCategory.label = label;
                detailCategory.rootCategoryName = modname;

                DefExplorerWindow.detailCategories.Add(subclassType, detailCategory);
                //DefDatabase<DetailCategory>.Add(detailCategory);
            }

            return detailCategory;
        }

        public static ItemDetail MakeItemDetail<T>(T def, DetailCategory category) where T : Def
        {
            var itemDef = new ItemDetail();
            itemDef.keyObject = def;
            if (def.label == null || def.label == "")
                itemDef.label = def.defName;
            else
                itemDef.label = def.LabelCap;
            itemDef.category = category;
            itemDef.description = def.description;

            DetailWrapper subDefSection = new DetailWrapper(def, category, true, null);
            itemDef.HelpDetailWrappers.Add(subDefSection);
            return itemDef;
        }
    }
}
