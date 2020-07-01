using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace DRimEditor.DetailView
{

	public class DetailCategory
	{
        public string label;

        public Type subclassType;

        public string rootCategoryName;
        public List<List<FieldInfo>> sectionFieldList;

        [Unsaved] 
        public List<ItemDetail> cachedItemDetails = new List<ItemDetail>();

        public DetailCategory(Type subclassType)
        {
            this.subclassType = subclassType;
            sectionFieldList = DatabaseHelper.GetSectionFieldList(this.subclassType);
        }

        public List<ItemDetail> ItemDetailDefs
        {
            get
            {
                return cachedItemDetails;
            }
        }

        public void Add(ItemDetail newDef)
        {
            cachedItemDetails.Add(newDef);
            cachedItemDetails.Sort();
        }

        public bool ShouldDraw { get; set; }

        public bool Expanded { get; set; }

        public bool MatchesFilter(string filter)
        {
            return filter == "" || label.ToString().ToUpper().Contains(filter.ToUpper());
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
            foreach (ItemDetail hd in ItemDetailDefs)
            {
                //hd.Filter(filter, force || MatchesFilter(filter));
                hd.Filter(filter, force);
            }
        }

        public void Recache()
        {
            cachedItemDetails.Clear();
            foreach (var def in (
                from t in DefExplorerWindow.itemDetails.Values
                where t.category == this
                select t))
            {
                cachedItemDetails.Add(def);
            }
            cachedItemDetails.Sort();

        }

        
    }
}
