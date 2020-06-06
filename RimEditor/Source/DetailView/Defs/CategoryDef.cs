using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace DRimEditor.DetailView
{
	// Token: 0x02000005 RID: 5
	public class CategoryDef : Def
	{

        public Type defType;

        public string ModName;
        public List<List<FieldInfo>> sectionFieldList;

        [Unsaved] 
        public List<ItemDetailDef> _cachedHelpDefs = new List<ItemDetailDef>();
        [Unsaved]
        public string keyDef;

        public CategoryDef(Type subclassType)
        {
            defType = subclassType;
            sectionFieldList = DatabaseBuilder.GetSectionFieldList(defType);
        }

        public List<ItemDetailDef> ItemDetailDefs
        {
            get
            {
                return _cachedHelpDefs;
            }
        }

        public void Add(ItemDetailDef newDef)
        {
            _cachedHelpDefs.Add(newDef);
            _cachedHelpDefs.Sort();
        }

        public bool ShouldDraw { get; set; }

        public bool Expanded { get; set; }

        public bool MatchesFilter(string filter)
        {
            return filter == "" || LabelCap.ToString().ToUpper().Contains(filter.ToUpper());
        }

        public bool ThisOrAnyChildMatchesFilter(string filter)
        {
            ShouldDraw = MatchesFilter(filter) || ItemDetailDefs.Any(hd => hd.MatchesFilter(filter));
            return ShouldDraw;
            //return MatchesFilter(filter) || HelpDefs.Any(hd => hd.MatchesFilter(filter));
        }

        public void Filter(string filter, bool force = false)
        {
            //ShouldDraw = force || ThisOrAnyChildMatchesFilter(filter);
            ShouldDraw = force || ShouldDraw;
            //Expanded = filter != "" && (force || ShouldDraw);
            Expanded = filter != "" && ShouldDraw;
            foreach (ItemDetailDef hd in ItemDetailDefs)
            {
                //hd.Filter(filter, force || MatchesFilter(filter));
                hd.Filter(filter, force);
            }
        }


        public override void ResolveReferences()
        {
            base.ResolveReferences();
            Recache();
        }

        public void Recache()
        {
            _cachedHelpDefs.Clear();
            foreach (var def in (
                from t in DefDatabase<ItemDetailDef>.AllDefs
                where t.category == this
                select t))
            {
                _cachedHelpDefs.Add(def);
            }
            _cachedHelpDefs.Sort();

        }

        
    }
}
