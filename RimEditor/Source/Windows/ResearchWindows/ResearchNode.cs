// ResearchNode.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DRimEditor.Windows;
using RimWorld;
using UnityEngine;
using Verse;
using static DRimEditor.Research.Constants;

namespace DRimEditor.Research
{
    [StaticConstructorOnStartup]
    public class ResearchNode : Node
    {
        public static Texture2D shardTex = ContentFinder<Texture2D>.Get("Things/Item/Special/Techshard/Techshard_c", true);

        private static readonly Dictionary<ResearchProjectDef, bool> _buildingPresentCache =
            new Dictionary<ResearchProjectDef, bool>();

        private static readonly Dictionary<ResearchProjectDef, List<ThingDef>> _missingFacilitiesCache =
            new Dictionary<ResearchProjectDef, List<ThingDef>>();

        public ResearchProjectDef Research;

        public ResearchNode( ResearchProjectDef research )
        {
            Research = research;

            // initialize position at vanilla y position, leave x at zero - we'll determine this ourselves
            _pos = new Vector2( 0, research.researchViewY + 1 );
        }

        public List<ResearchNode> Parents
        {
            get
            {
                var parents = InNodes.OfType<ResearchNode>();
                parents.Concat( InNodes.OfType<DummyNode>().Select( dn => dn.Parent ) );
                return parents.ToList();
            }
        }

        public override Color Color
        {
            get
            {
                if (ResearchSelectPanel.selected == this)
                    return new Color(249f/255f, 146f/255f, 69f/255f);
                if (DarkHighlighted)
                    return GenUI.MouseoverColor;
                if ( Highlighted )
                    return GenUI.SubtleMouseoverColor;
                if ( Completed )
                    return Assets.ColorCompleted[Research.techLevel];
                if ( Available )
                    return Assets.ColorCompleted[Research.techLevel];
                return Assets.ColorUnavailable[Research.techLevel];
            }
        }

        public override Color EdgeColor
        {
            get
            {
                if (ResearchSelectPanel.selected == this)
                    return new Color(249f / 255f, 146f / 255f, 69f / 255f);
                if (DarkHighlighted)
                    return GenUI.MouseoverColor;
                if ( Highlighted )
                    return GenUI.MouseoverColor;
                if ( Completed )
                    return Assets.ColorCompleted[Research.techLevel];
                if ( Available )
                    return Assets.ColorAvailable[Research.techLevel];
                return Assets.ColorUnavailable[Research.techLevel];
            }
        }

        public List<ResearchNode> Children
        {
            get
            {
                var children = OutNodes.OfType<ResearchNode>() ?? new List<ResearchNode>();
                children = children.Concat( OutNodes.OfType<DummyNode>().Select( dn => dn.Child ) );

                return children.ToList();
            }
        }

        public List<ResearchNode> AllChildren
        {
            get
            {
                var children = Children;
                if (children == null)
                    return new List<ResearchNode>();
                var allChildren = new List<ResearchNode>(children);
                foreach (var child in children)
                    allChildren.AddRange(child.AllChildren);

                return allChildren.Distinct().ToList();
            }
        }

        public List<Edge<Node,Node>> GetPathTo(ResearchNode rn, List<Edge<Node,Node>> edges)
        {
            List<Edge<Node, Node>> path;
            path = new List<Edge<Node, Node>>();
            foreach (var edge in edges)
            {
                if (edge.In == rn)
                {
                    path.Add(edge);
                    return path;
                }
                else if (!(edge.In is DummyNode))
                {
                    continue;
                }
                List<Edge<Node, Node>> found = GetPathTo(rn, edge.In.InEdges);
                if (found != null)
                {
                    path.Add(edge);
                    path.AddRange(found);
                    return path;
                }
            }
            return null;
        }

        
        public void RemovePrereq(ResearchNode rn)
        {
            var path = GetPathTo(rn, _inEdges);
            foreach(var edge in path ?? Enumerable.Empty<Edge<Node, Node>>())
            {
                //Tree._edges.Remove(Tree._edges.Find(x => x.In == edge.In && x.Out == edge.Out));
                Tree._edges.Remove(edge);
                edge.Out._inEdges.Remove(edge);
                edge.In._outEdges.Remove(edge);
                if (edge.Out is DummyNode && edge.Out.OutEdges.Count == 0 && edge.Out.InEdges.Count == 0)
                    Tree._nodes.Remove(edge.Out);
            }
        }

        public void AddPrereq(ResearchNode rn)
        {
            var newEdge = new Edge<Node, Node>(rn, this);
            Tree._edges.Add(newEdge);
            _inEdges.Add(newEdge);
            rn._outEdges.Add(newEdge);
        }


        public override string Label => Research.LabelCap;

        public static bool BuildingPresent( ResearchProjectDef research )
        {
            if ( DebugSettings.godMode && Prefs.DevMode )
                return true;

            // try get from cache
            bool result;
            if ( _buildingPresentCache.TryGetValue( research, out result ) )
                return result;

            // do the work manually
            if ( research.requiredResearchBuilding == null )
                result = true;
            else
                result = Find.Maps.SelectMany( map => map.listerBuildings.allBuildingsColonist )
                             .OfType<Building_ResearchBench>()
                             .Any( b => research.CanBeResearchedAt( b, true ) );

            if ( result )
                result = research.Ancestors().All( BuildingPresent );

            // update cache
            _buildingPresentCache.Add( research, result );
            return result;
        }

        public static bool TechprintAvailable( ResearchProjectDef research )
        {
            return research.TechprintRequirementMet;
        }

        public static void ClearCaches()
        {
            _buildingPresentCache.Clear();
            _missingFacilitiesCache.Clear();
        }

        public static implicit operator ResearchNode( ResearchProjectDef def )
        {
            return def.ResearchNode();
        }

        public int Matches( string query )
        {
            var culture = CultureInfo.CurrentUICulture;
            query = query.ToLower( culture );

            if ( Research.LabelCap.RawText.ToLower( culture ).Contains( query ) )
                return 1;
            if ( Research.GetUnlockDefsAndDescs()
                         .Any( unlock => unlock.First.LabelCap.RawText.ToLower( culture ).Contains( query ) ) )
                return 2;
            if ( Research.description.ToLower( culture ).Contains( query ) )
                return 3;
            return 0;
        }

        public bool BuildingPresent()
        {
            return BuildingPresent( Research );
        }
        
        public bool TechprintAvailable()
        {
            return TechprintAvailable( Research );
        }


        public void DoDraw(Rect visibleRect, bool forceDetailedMode = false, bool notDrawingSelected = true)
        {
            if (!IsVisible(visibleRect))
            {
                Highlighted = false;
                DarkHighlighted = false;
                return;
            }

            var detailedMode = forceDetailedMode ||
                               ResearchWindow.Instance.ZoomLevel < DetailedModeZoomLevelCutoff;
            var mouseOver = Mouse.IsOver(Rect);
            if (Event.current.type == EventType.Repaint)
            {
                // researches that are completed or could be started immediately, and that have the required building(s) available
                //GUI.color = mouseOver && ResearchSelectPanel.selected != this ? GenUI.MouseoverColor : Color;
                GUI.color = Color;

                if (notDrawingSelected && (mouseOver || Highlighted))
                    GUI.DrawTexture(Rect, Assets.ButtonActive);
                else
                    GUI.DrawTexture(Rect, Assets.Button);

                // grey out center to create a progress bar effect, completely greying out research not started.
                var progressBarRect = Rect.ContractedBy(3f);
                GUI.color = Assets.ColorAvailable[Research.techLevel];
                progressBarRect.xMin += progressBarRect.width;
                GUI.DrawTexture(progressBarRect, BaseContent.WhiteTex);

                Highlighted = false;
                DarkHighlighted = false;

                GUI.color = Color.white;

                if (detailedMode)
                {
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.WordWrap = false;
                    Text.Font = _largeLabel && notDrawingSelected ? GameFont.Tiny : GameFont.Small;
                    Widgets.Label(LabelRect, Research.LabelCap);
                }
                else
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.WordWrap = false;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(Rect, Research.LabelCap);
                }

                // draw research cost and icon
                if (detailedMode)
                {
                    Text.Anchor = TextAnchor.UpperRight;
                    Text.Font = Research.baseCost > 1000000 ? GameFont.Tiny : GameFont.Small;
                    if (Research.techprintCount == 0)
                    {
                        Widgets.Label(CostLabelRect, Research.baseCost.ToStringByStyle(ToStringStyle.Integer));
                        GUI.DrawTexture(CostIconRect, Assets.ResearchIcon, ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        int printsNeeded = Research.techprintCount;
                        string label = printsNeeded.ToString();
                        Text.Font = GameFont.Tiny;
                        Widgets.Label(CostLabelRect, label);
                        GUI.DrawTexture(CostIconRect, shardTex, ScaleMode.ScaleToFit);
                        if (!notDrawingSelected)
                        {
                            Rect selectedCostIconRect = new Rect(CostIconRect.x, CostIconRect.yMax + 2f, CostIconRect.width, CostIconRect.height);
                            GUI.DrawTexture(selectedCostIconRect, Assets.ResearchIcon, ScaleMode.ScaleToFit);
                            Rect selectedCostLabelRect = new Rect(CostLabelRect.x, selectedCostIconRect.y, CostLabelRect.width, CostLabelRect.height);
                            Widgets.Label(selectedCostLabelRect, Research.baseCost.ToStringByStyle(ToStringStyle.Integer));
                        }
                    }
                }

                Text.WordWrap = true;

                // attach description and further info to a tooltip
                TooltipHandler.TipRegion(Rect, GetResearchTooltipString, Research.GetHashCode());
                /* if ( !BuildingPresent() )
                 {
                     TooltipHandler.TipRegion( Rect,
                         ResourceBank.String.MissingFacilities( string.Join( ", ",
                             MissingFacilities().Select( td => td.LabelCap ).ToArray() ) ) );
                 } else if (!TechprintAvailable()) {
                     TooltipHandler.TipRegion(Rect,
                         ResourceBank.String.MissingTechprints(Research.TechprintsApplied, Research.techprintCount));
                 }
                 */

                // draw unlock icons
                if (detailedMode && notDrawingSelected)
                {
                    var unlocks = Research.GetUnlockDefsAndDescs();
                    for (var i = 0; i < unlocks.Count; i++)
                    {
                        var iconRect = new Rect(
                            IconsRect.xMax - (i + 1) * (IconSize.x + 4f),
                            IconsRect.yMin + (IconsRect.height - IconSize.y) / 2f,
                            IconSize.x,
                            IconSize.y);

                        if (iconRect.xMin - IconSize.x < IconsRect.xMin &&
                             i + 1 < unlocks.Count)
                        {
                            // stop the loop if we're about to overflow and have 2 or more unlocks yet to print.
                            iconRect.x = IconsRect.x + 4f;
                            GUI.DrawTexture(iconRect, Assets.MoreIcon, ScaleMode.ScaleToFit);
                            var tip = string.Join("\n",
                                                   unlocks.GetRange(i, unlocks.Count - i).Select(p => p.Second)
                                                          .ToArray());
                            TooltipHandler.TipRegion(iconRect, tip);
                            // new TipSignal( tip, Settings.TipID, TooltipPriority.Pawn ) );
                            break;
                        }

                        // draw icon
                        unlocks[i].First.DrawColouredIcon(iconRect);

                        // tooltip
                        TooltipHandler.TipRegion(iconRect, unlocks[i].Second);
                    }
                }
            }
            else if (mouseOver)
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && ResearchSelectPanel._instance.draggingDef != null && ResearchSelectPanel.selected != this)
                {
                    ResearchProjectEditor.SwapUnlockable(ResearchSelectPanel.selected, this, ResearchSelectPanel._instance.draggingDef.def);
                }
            }

            // if clicked and not yet finished, queue up this research and all prereqs.
            if (Widgets.ButtonInvisible(Rect))
            {
                DoClick();
            }
        }

        public void DoClick()
        {
            if (Event.current.button == 0)
            {
                if (ResearchSelectPanel.selected == this)
                {
                    ResearchWindow.Instance.CenterOn(this);
                }
                else
                {
                    ResearchSelectPanel.Select(this);
                }
            }
            else if (Event.current.button == 1)
            {
                var options = new List<FloatMenuOption>();
                options.Add(new FloatMenuOption("Open in Def Explorer...", () => ClickHandler(), MenuOptionPriority.High, null));
                if (ResearchSelectPanel.selected != null)
                {
                    if (ResearchSelectPanel.selected == this)
                    {
                        
                    }
                    else
                    {
                        if (GetPrereqs().Contains(ResearchSelectPanel.selected))
                        { // this is a direct child for selected, can remove child
                            options.Add(new FloatMenuOption("Remove Child from " + ResearchSelectPanel.selected.Label, () => ResearchProjectEditor.RemoveChild(node: ResearchSelectPanel.selected, child: this), MenuOptionPriority.Default, null));
                        }
                        else if (!AllChildren.Contains(ResearchSelectPanel.selected)) // this is not a prereq for selected or a direct child, can add child
                        {
                            options.Add(new FloatMenuOption("Add Child to " + ResearchSelectPanel.selected.Label, () => ResearchProjectEditor.AddChild(node: ResearchSelectPanel.selected, child: this), MenuOptionPriority.Default, null));
                        }
                        else // this is a prereq, can't add child
                        {
                            options.Add(new FloatMenuOption("(Can't add child, would create cycle)", null, MenuOptionPriority.Default, null));
                        }

                        if (Children.Contains(ResearchSelectPanel.selected)) // this is a direct prereq for selected, can remove prereq
                        {
                            options.Add(new FloatMenuOption("Remove Prerequisite from " + ResearchSelectPanel.selected.Label, () => ResearchProjectEditor.RemovePrereq(node: ResearchSelectPanel.selected, prereq: this), MenuOptionPriority.Default, null));
                        }
                        else if (!GetPrereqsRecursive().Contains(ResearchSelectPanel.selected)) // this is not a child for selected or a direct prereq, can add prereq
                        {
                            options.Add(new FloatMenuOption("Add Prerequisite to " + ResearchSelectPanel.selected.Label, () => ResearchProjectEditor.AddPrereq(node: ResearchSelectPanel.selected, prereq: this), MenuOptionPriority.Default, null));
                        }
                        else
                        {
                            options.Add(new FloatMenuOption("(Can't add prerequisite, would create cycle)", null, MenuOptionPriority.High, null));
                        }

                    }


                }
                options.Add(new FloatMenuOption("Delete Research Project", () => ResearchProjectEditor.Delete(this), MenuOptionPriority.Low, null));
                /*
                if (options.Count == 0) // can't do anything with this KEKWait
                {
                    options.Add(new FloatMenuOption("(No actions available)", null, MenuOptionPriority.Default, null));
                }
                */
                options.Add(new FloatMenuOption("Set tech level...", () => ResearchProjectEditor.SetTechLevel(this), MenuOptionPriority.Low, null));
                options.Add(new FloatMenuOption("Set techprint count...", () => ResearchProjectEditor.SetTechprintCount(this), MenuOptionPriority.Low, null));
                Find.WindowStack.Add(new FloatMenu(options));
                //Event.current.Use();
            }
        }

        public void ClickHandler()
        {
            //MainWindow.Open(MainWindow.defExplorerWindow);
            MainWindow.defExplorerWindow.JumpTo(Research);
        }

        /// <summary>
        ///     Draw the node, including interactions.
        /// </summary>
        public override void Draw( Rect visibleRect, bool forceDetailedMode = false)
        {
            DoDraw(visibleRect, forceDetailedMode);
        }

        /// <summary>
        ///     Get recursive list of all incomplete prerequisites
        /// </summary>
        /// <returns>List<Node> prerequisites</Node></returns>
        public List<ResearchNode> GetPrereqsRecursive()
        {
            var parents = Research.prerequisites?.Select( rpd => rpd.ResearchNode() );
            if ( parents == null )
                return new List<ResearchNode>();
            var allParents = new List<ResearchNode>( parents );
            foreach ( var parent in parents )
                allParents.AddRange( parent.GetPrereqsRecursive() );

            return allParents.Distinct().ToList();
        }

        public List<ResearchNode> GetPrereqs()
        {
            var parents = Research.prerequisites?.Select(rpd => rpd.ResearchNode());
            if (parents == null)
                return new List<ResearchNode>();
            return new List<ResearchNode>(parents);
        }

        public override bool Completed => true;
        public override bool Available => true;

        /// <summary>
        ///     Creates text version of research description and additional unlocks/prereqs/etc sections.
        /// </summary>
        /// <returns>string description</returns>
        private string GetResearchTooltipString()
        {
            // start with the descripton
            var text = new StringBuilder();
            text.AppendLine( Research.description );
            text.AppendLine();

            if ( DebugSettings.godMode )
            {
                text.AppendLine( ResourceBank.String.CLClickDebugInstant );
            }

            return text.ToString();
        }

        public void DrawAt( Vector2 pos, Rect visibleRect, bool forceDetailedMode = false )
        {
            SetRects( pos );
            DoDraw( visibleRect, forceDetailedMode, false);
            SetRects();
        }
    }
}