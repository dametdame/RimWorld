using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRimEditor.DetailView
{
    public class RootCategory
    {
        readonly List<DetailCategory> detailCategories = new List<DetailCategory>();
        public readonly string rootName;
        public bool Expanded;

        public RootCategory(string rootName)
        {
            this.rootName = rootName;
        }

        public List<DetailCategory> DetailCategories
        {
            get
            {
                return detailCategories.OrderBy(a => a.label).ToList();
            }
        }

        public bool ShouldDraw
        {
            get;
            set;
        }

        public bool MatchesFilter(string filter)
        {
            return (
                (filter == "") ||
                (rootName.ToUpper().Contains(filter.ToUpper()))
            );
        }

        public bool ThisOrAnyChildMatchesFilter(string filter)
        {
            return (
                (MatchesFilter(filter)) ||
                (DetailCategories.Any(hc => hc.ThisOrAnyChildMatchesFilter(filter)))
            );
        }

        public void Filter(string filter)
        {
            ShouldDraw = ThisOrAnyChildMatchesFilter(filter);
            Expanded = (
                (filter != "") &&
                (ShouldDraw)
            );

            foreach (DetailCategory dc in DetailCategories)
            {
                dc.Filter(filter, MatchesFilter(filter));
            }
        }

        public void AddCategory(DetailCategory def)
        {
            detailCategories.AddUnique(def);
        }
    }
}
