using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Xml;

namespace DArcaneTechnology
{
    public class ExemptResearch : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            foreach (string s in Exemptions)
            {
                if (!DArcaneTechnology.GearAssigner.exemptProjects.Contains(s))
                    DArcaneTechnology.GearAssigner.exemptProjects.Add(s);
            }
            return true;
        }

        public List<string> Exemptions;
    }
}