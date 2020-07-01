using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DSkillPoints
{
    public class CompSkillPoints : ThingComp
    {
		public CompProperties_SkillPoints Props
		{
			get
			{
				return this.props as CompProperties_SkillPoints;
			}
		}


        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

			var inspectTabs = parent.GetInspectTabs();
			if (inspectTabs != null && inspectTabs.Count() > 0 && inspectTabs.FirstOrDefault(x => x is ITab_Skills) == null)
            {
				int bioIndex = inspectTabs.FirstIndexOf(x => x is ITab_Pawn_Character);
				int skillIndex = Math.Min(bioIndex + 1, inspectTabs.Count());
				try
				{
					parent.def.inspectorTabsResolved.Insert(skillIndex, InspectTabManager.GetSharedInstance(typeof(ITab_Skills)));
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
							"Could not instantiate inspector tab of type ",
							typeof(ITab_Skills),
							": ",
							ex
					}), false);
				}
				
			}
			
		}

        public override void PostExposeData()
		{
			base.PostExposeData();
			
		}


	}
}
