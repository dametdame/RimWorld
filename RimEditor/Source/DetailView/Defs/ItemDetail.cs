using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace DRimEditor.DetailView
{
	public class ItemDetail : IComparable
	{
        public string label;
        public string description;

        public DetailCategory category;

        [Unsaved]
        public object keyObject;

        public int CompareTo(object obj)
        {
            var d = obj as ItemDetail;
            return
                (d != null)
                ? string.Compare(d.label, label) * -1
                : 1;
        }

        public List<DetailWrapper> HelpDetailWrappers = new List<DetailWrapper>();

        public bool ShouldDraw { get; set; }

        public void Filter(string filter, bool force = false)
        {
            ShouldDraw = force || MatchesFilter(filter);
        }

        public bool MatchesFilter(string filter)
        {
            return filter == "" || (label != null && label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) || (keyObject is Def keyDef && keyDef.defName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

    }
}
