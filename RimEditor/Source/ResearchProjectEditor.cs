using DRimEditor.Research;
using DRimEditor.ResearchPal.RPUtility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static DRimEditor.Research.Constants;
using static DRimEditor.Extensions;
using HarmonyLib;
using DRimEditor.DetailView;
using DRimEditor.Windows.Dialogs;

namespace DRimEditor
{
    public static class ResearchProjectEditor
    {

        public static void RemoveChild(ResearchNode node, ResearchNode child)
        {
            RemovePrereq(child, node);
        }

        public static void RemovePrereq(ResearchNode node, ResearchNode prereq)
        {
            node.RemovePrereq(prereq);
            node.Research.prerequisites.Remove(prereq.Research);
            ProfileManager.AddCommand("".Find(node.Research).Get(new { node.Research.prerequisites }).Remove().Find(prereq.Research));
            //Research.ResearchViewWindow.Refresh();
        }

        public static void AddChild(ResearchNode node, ResearchNode child)
        {
            AddPrereq(child, node);
        }

        public static void AddPrereq(ResearchNode node, ResearchNode prereq)
        {
            if(node.Research.prerequisites == null)
            {
                node.Research.prerequisites = new List<ResearchProjectDef>();
                ProfileManager.AddCommand(Find(node.Research).Set(new { node.Research.prerequisites }).New(typeof(List<ResearchProjectDef>)));
            }
            node.Research.prerequisites.Add(prereq.Research);
            ProfileManager.AddCommand(Find(node.Research).Get(new { node.Research.prerequisites}).Add().Find(prereq.Research));
            node.AddPrereq(prereq);
            //Research.ResearchViewWindow.Refresh();
        }
        
        public static void MakeNewNode(Vector2 location)
        {
            var dialog = new Dialog_GiveResearchName(location);
            Verse.Find.WindowStack.Add(dialog);
        }

        public static void AddResearchProject(Vector2 location, string defName)
        {
            float x = (location.x + (NodeSize.x + NodeMargins.x)) / (NodeSize.x + NodeMargins.x);
            float y = (location.y + (NodeSize.y + NodeMargins.y)) / (NodeSize.y + NodeMargins.y);

            ResearchProjectDef newDef = new ResearchProjectDef();
            newDef.defName = defName;
            newDef.label = defName;
            newDef.description = defName;
            newDef.baseCost = 600;
            newDef.techLevel = TechLevel.Industrial;
            newDef.researchViewX = Rand.Range(0, 500)/100f;
            newDef.researchViewY = Rand.Range(0, 500)/100f;
            newDef.tab = ResearchTabDefOf.Main;
            ProfileManager.AddCommand(Enqueue().New(typeof(ResearchProjectDef)));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.defName }).Find(defName));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.label }).Find(defName));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.description }).Find(defName));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.baseCost }).Find(newDef.baseCost));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.techLevel }).Find(TechLevel.Industrial));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.researchViewX }).Find(newDef.researchViewX));
            ProfileManager.AddCommand(Enqueue().Dequeue().Set(new { newDef.tab }).Find(newDef.tab));
            ProfileManager.AddCommand(Add().Dequeue().Set(new { newDef.researchViewY }).Find(newDef.researchViewY));

            DefDatabase<ResearchProjectDef>.Add(newDef);

            //we're refreshing now smilers
            //wait no we're not KEKWait
            ResearchNode node = new ResearchNode(newDef);
            node.X = Mathf.RoundToInt(x);
            node.Y = Mathf.RoundToInt(y);
            DRimEditor.Research.Tree._nodes.Add(node);
            
            //ResearchWindow.Refresh();
            //ResearchNode sel = Research.Tree.Nodes.Find(n => n is ResearchNode rn && rn.Research == newDef) as ResearchNode;
            ResearchSelectPanel.Select(node);
            ResearchSelectPanel.newName = newDef.label;
            //DefExplorerWindow.Recache();
            if (DefExplorerWindow.initialized)
            {
                //DetailCategory cat = DefDatabase<DetailCategory>.AllDefsListForReading.Find(c => c.defType == typeof(ResearchProjectDef));
                DetailCategory cat = DefExplorerWindow.detailCategories[typeof(ResearchProjectDef)];
                DefExplorerWindow.instance.AddNewDefToCat(newDef, cat);
            }
        }

        public static void SetName(ResearchNode node, string newName)
        {
            node.Research.label = newName;
            var cachedLabelCap = AccessTools.Field(typeof(Def), "cachedLabelCap");
            cachedLabelCap.SetValue(node.Research, null);
            ProfileManager.AddCommand("".Find(node.Research).Set(new { node.Research.label }).Find(newName));
        }

        public static void AddUnlockable(ResearchNode node, Def unlockable = null)
        {
            Def toAdd = unlockable;
            if (toAdd == null)
            {
                // get a new unlockable here
            }
            if (toAdd is ThingDef curthing)
            {
                if (curthing.plant?.sowResearchPrerequisites != null)
                {
                    curthing.plant.sowResearchPrerequisites.Add(node.Research);
                    ProfileManager.AddCommand("".Find(curthing).Get(new { curthing.plant }).Get(new { curthing.plant.sowResearchPrerequisites }).Add().Find(node.Research));
                    ResearchSelectPanel.unlockedDefs.Clear();
                }
                else if (curthing.researchPrerequisites != null)
                {
                    curthing.researchPrerequisites.Add(node.Research);
                    ProfileManager.AddCommand("".Find(curthing).Get(new { curthing.researchPrerequisites }).Add().Find(node.Research));
                }
            }
            else if (toAdd is TerrainDef curterrain && (curterrain.researchPrerequisites != null))
            {
                curterrain.researchPrerequisites.Add(node.Research);
                ProfileManager.AddCommand("".Find(curterrain).Get(new { curterrain.researchPrerequisites }).Add().Find(node.Research));
            }
            else if (toAdd is RecipeDef currecipe)
            {
                currecipe.researchPrerequisite = node.Research;
                ProfileManager.AddCommand(Find(currecipe).Set(new { currecipe.researchPrerequisite }).Find(node.Research));
                if (currecipe.ProducedThingDef != null && currecipe.ProducedThingDef.recipeMaker != null)
                {
                    currecipe.ProducedThingDef.recipeMaker.researchPrerequisite = node.Research;
                    ProfileManager.AddCommand(Find(currecipe.ProducedThingDef).Get(new { currecipe.ProducedThingDef.recipeMaker }).Set(new { currecipe.ProducedThingDef.recipeMaker.researchPrerequisite } ).Find(node.Research));
                }
            }
            else
            {
                Verse.Log.Error("RimEdit: Add unlockable but not thing, terrain, or recipe with this rpd as prereq");
            }
            ResearchProjectDef_Extensions.ClearCache();
            ResearchSelectPanel.unlockedDefs.Clear();
            ResearchSelectPanel.unlockedInit = false;
        }

        public static void RemoveUnlockable(ResearchNode from, Def curDef)
        {
            if (curDef is ThingDef curthing)
            {
                if (curthing.plant?.sowResearchPrerequisites?.Contains(from.Research) ?? false)
                {
                    curthing.plant.sowResearchPrerequisites.Remove(from.Research);
                    ProfileManager.AddCommand("".Find(curthing).Get(new { curthing.plant }).Get(new { curthing.plant.sowResearchPrerequisites}).Remove().Find(from.Research));
                }
                else if (curthing.researchPrerequisites?.Contains(from.Research) ?? false)
                {
                    curthing.researchPrerequisites.Remove(from.Research);
                    ProfileManager.AddCommand("".Find(curthing).Get(new { curthing.researchPrerequisites }).Remove().Find(from.Research));
                }
            }
            else if (curDef is TerrainDef curterrain && (curterrain.researchPrerequisites?.Contains(from.Research) ?? false))
            {
                curterrain.researchPrerequisites.Remove(from.Research);
                ProfileManager.AddCommand("".Find(curterrain).Get(new { curterrain.researchPrerequisites }).Remove().Find(from.Research));
            }
            else if (curDef is RecipeDef currecipe && currecipe.researchPrerequisite == from.Research)
            {
                currecipe.researchPrerequisite = null;
                ProfileManager.AddCommand(Find(currecipe).Set(new { currecipe.researchPrerequisite }).FindNull());
                if (currecipe.ProducedThingDef != null && currecipe.ProducedThingDef.recipeMaker != null)
                {
                    currecipe.ProducedThingDef.recipeMaker.researchPrerequisite = null;
                    ProfileManager.AddCommand(Find(currecipe.ProducedThingDef).Get(new { currecipe.ProducedThingDef.recipeMaker }).Set(new { currecipe.ProducedThingDef.recipeMaker.researchPrerequisite }).FindNull());
                }
            }
            else
            {
                Verse.Log.Error("RimEdit: Remove unlockable but not thing, terrain, or recipe with this rpd as prereq");
            }
            ResearchProjectDef_Extensions.ClearCache();
            ResearchSelectPanel.unlockedDefs.Clear();
            ResearchSelectPanel.unlockedInit = false;
        }

        public static void SwapUnlockable(ResearchNode from, ResearchNode to, Def unlockable)
        {
            RemoveUnlockable(from, unlockable);
            AddUnlockable(to, unlockable);
        }

        public static void Delete(ResearchNode node)
        {
            if (ResearchSelectPanel.selected == node)
            {
                ResearchSelectPanel.Unselect();
            }
            foreach (var unlocked in node.Research.GetDirectUnlocks() ?? Enumerable.Empty<Pair<Def, string>>())
            {
                RemoveUnlockable(node, unlocked.First);
            }
            foreach(var child in node.Children ?? Enumerable.Empty<ResearchNode>())
            {
                RemoveChild(node, child);
            }
            foreach(var prereq in node.GetPrereqs() ?? Enumerable.Empty<ResearchNode>())
            {
                RemovePrereq(node, prereq);
            }
            var r = AccessTools.Method(typeof(DefDatabase<ResearchProjectDef>), "Remove");
            r.Invoke(null, new object[] { node.Research });
            ProfileManager.AddCommand(Remove().Find(node.Research));
            Research.Tree._nodes.Remove(node);
            //Research.ResearchWindow.Refresh();
        }

        public static void SetTechLevel(ResearchNode node)
        {
            List<string> techLevels = new List<string>(Enum.GetNames(typeof(TechLevel)));
            techLevels.Remove("Undefined");
            var dialog = new Dialog_TechLevels(techLevels, node);
            Verse.Find.WindowStack.Add(dialog);
        }

        public static void SetTechprintCount(ResearchNode node)
        {
            var dialog = new Dialog_SetTechprintCount(node);
            Verse.Find.WindowStack.Add(dialog);
        }

        public static void DoSetTechprintCount(ResearchNode node, int count)
        {
            node.Research.techprintCount = count;
            ProfileManager.AddCommand(Find(node.Research).Set(new { node.Research.techprintCount }).Find(count));
        }

        public static void DoSetTechLevel(ResearchNode node, string tech)
        {
            TechLevel techlevel = (TechLevel)Enum.Parse(typeof(TechLevel), tech);
            node.Research.techLevel = techlevel;
            ProfileManager.AddCommand(Find(node.Research).Set(new { node.Research.techLevel }).Find(techlevel));
        }

    }

    public class Dialog_TechLevels : Dialog_ListSelect
    {
        ResearchNode node;

        public Dialog_TechLevels(List<string> techLevels, ResearchNode rn) : base(techLevels)
        {
            node = rn;
        }

        public override void DoAccept(string option)
        {
            ResearchProjectEditor.DoSetTechLevel(node, option);
        }

    }

    public class Dialog_SetTechprintCount : SimpleTextEntryDialog
    {
        ResearchNode researchNode;
        int count;

        public Dialog_SetTechprintCount(ResearchNode rn) : base("Enter new techprint count")
        {
            researchNode = rn;
        }

        protected override bool IsValid(string s)
        {
            int parsed;
            if (Int32.TryParse(s, out parsed) && parsed >= 0)
            {
                count = parsed;
                return true;
            }
            return false;
        }

        protected override void DoAccept(string s)
        {
            ResearchProjectEditor.DoSetTechprintCount(researchNode, count);
        }
    }

    public class Dialog_GiveResearchName : SimpleTextEntryDialog
    {
        Vector2 location;

        public Dialog_GiveResearchName(Vector2 loc) : base()
        {
            location = loc;
        }

        protected override bool IsValid(string s)
        {
            return s.Split(' ').Count() == 1 && DefDatabase<ResearchProjectDef>.GetNamedSilentFail(s) == null;
        }

        protected override void DoAccept(string s)
        {
            ResearchProjectEditor.AddResearchProject(location, s);
        }

        
    }

   
}
