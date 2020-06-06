using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace DRimEditor.DetailView
{
	public class ItemDetailDef : Def, IComparable
	{

        public CategoryDef category;

        [Unsaved]
        public Def keyDef;
        [Unsaved]
        public Def secondaryKeyDef;

        public int CompareTo(object obj)
        {
            var d = obj as ItemDetailDef;
            return
                (d != null)
                ? string.Compare(d.label, label) * -1
                : 1;
        }

        public List<DetailWrapper> HelpDetailWrappers = new List<DetailWrapper>();

        public string Description
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine(description);
                foreach (DetailWrapper section in HelpDetailWrappers)
                {
                    s.AppendLine(section.GetString());
                }
                return s.ToString();
            }
        }

        public bool ShouldDraw { get; set; }

        public void Filter(string filter, bool force = false)
        {
            ShouldDraw = force || MatchesFilter(filter);
        }

        public bool MatchesFilter(string filter)
        {
            return filter == "" || LabelCap != null && LabelCap.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }
}
