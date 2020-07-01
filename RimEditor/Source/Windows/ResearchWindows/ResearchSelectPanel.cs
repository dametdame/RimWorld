// Queue.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using DRimEditor.ResearchPal.RPUtility;
using static DRimEditor.Research.Assets;
using static DRimEditor.Research.Constants;
using UnityEngine.UIElements;

namespace DRimEditor.Research
{
    public class ResearchSelectPanel
    {
        public static ResearchSelectPanel _instance;
        public static ResearchNode selected;

        public Vector2 _mousePosition = Vector2.zero;
        public int dragNum = -1;
        public UnlockedDef draggingDef = null;

        public static List<UnlockedDef> unlockedDefs = new List<UnlockedDef>();
        public static bool unlockedInit = false;

        public static string newName;


        public ResearchSelectPanel()
        {
            _instance = this;
        }

        /*
        public static void DrawLabels(Rect visibleRect)
        {
            
            var i = 1;
            foreach (var node in _instance._queue)
            {
                if (node.IsVisible(visibleRect))
                {
                    var main = ColorCompleted[node.Research.techLevel];
                    var background = i > 1 ? ColorUnavailable[node.Research.techLevel] : main;
                    DrawLabel(node.QueueRect, main, background, i);
                }

                i++;
            }

        }

        
        public static void DrawLabel(Rect canvas, Color main, Color background, int label)
        {
            // draw coloured tag
            GUI.color = main;
            GUI.DrawTexture(canvas, CircleFill);

            // if this is not first in line, grey out centre of tag
            if (background != main)
            {
                GUI.color = background;
                GUI.DrawTexture(canvas.ContractedBy(2f), CircleFill);
            }

            // draw queue number
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(canvas, label.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
        }*/

        public static void Unselect()
        {
            selected = null;
            newName = null;
            unlockedDefs.Clear();
            unlockedInit = false;
        }

        public static void Select(ResearchNode node)
        {
            selected = node;
            newName = node.Research.label;
            unlockedDefs.Clear();
            unlockedInit = false;
        }


        public static void DrawPanel(Rect canvas, bool interactible)
        {
            //if (!_instance._queue.Any())
            {
                //Text.Anchor = TextAnchor.MiddleCenter;
                //GUI.color = TechLevelColor;
                //Widgets.Label(canvas, ResourceBank.String.NothingQueued);
                //Text.Anchor = TextAnchor.UpperLeft;
                //GUI.color = Color.white;
                //return;
            }

           

            if (selected != null)
            {
                //foreach (var prerequisite in selected.GetPrereqsRecursive())
                //    prerequisite.Highlighted = true;
                foreach (var prerequisite in selected.GetPrereqs())
                    prerequisite.DarkHighlighted = true;

               // foreach (var child in selected.AllChildren)
               //     child.Highlighted = true;
                foreach (var child in selected.Children)
                    child.DarkHighlighted = true;

                Vector2 pos = canvas.min;
                Rect selectedRect = new Rect(
                    pos.x + Margin,
                    pos.y,
                    NodeSize.x*2 + 3 * Margin,
                    NodeSize.y*2 + 4 * Margin
                );

                DoSelectedView(selectedRect);

                if (!unlockedInit)
                {
                    InitUnlockedIcons(selected, canvas);
                }

                if (interactible)
                {
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                    {
                        GUI.FocusControl(null);
                    }
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                    {
                        _instance._mousePosition = Vector2.zero;
                        _instance.dragNum = -1;
                        _instance.draggingDef = null;
                    }
                }

                foreach (UnlockedDef unlocked in unlockedDefs)
                {
                    unlocked.Draw();
                }
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.IsOver(canvas))
            {
                Event.current.Use();
            }
        }

        public static void DoSelectedView(Rect canvas)
        {
            Rect selectedRect = new Rect(
                    canvas.xMin,
                    canvas.yMin,
                    NodeSize.x + 2 * Margin,
                    NodeSize.y + 2 * Margin
                );
            selected.DrawAt(selectedRect.min, selectedRect, true);
            float infoHeight = NodeSize.y / 3f;

            Rect editRect = new Rect(selectedRect.xMax + Margin, canvas.yMin, NodeSize.x, NodeSize.y + Margin * 2);
            Rect nameRect = editRect.TopPartPixels(infoHeight);
            newName = Widgets.TextField(nameRect, newName);
            Rect labelRect = new Rect(editRect);
            labelRect.y = labelRect.y + infoHeight + Margin / 2;
            labelRect.x = editRect.xMin;
            labelRect.height = infoHeight + Margin;
            GUI.Label(labelRect, selected.Research.defName);
            Rect buttonRect = editRect.BottomPartPixels(infoHeight);
            buttonRect.width /= 3f;
            bool saveClicked = Widgets.ButtonText(buttonRect, "Save");
            Rect newUnlockRect = new Rect(buttonRect.xMax + Margin, buttonRect.yMin, editRect.width * 2f/3f - Margin, buttonRect.height);
            bool newClicked = Widgets.ButtonText(newUnlockRect, "Add unlockable...");

            if (saveClicked)
            {
                ResearchProjectEditor.SetName(selected, newName);
            }
            if (newClicked)
            {
                ResearchProjectEditor.AddUnlockable(selected);
            }
        }

        public static void InitUnlockedIcons(ResearchNode node, Rect canvas, bool bigIcons = true)
        {
            Vector2 iconSize = bigIcons ? bigIconSize : IconSize;
            float iconSpace = (iconSize.x + 4f);
            float defaultX = canvas.xMin + NodeSize.x * 2f + Margin + iconSpace;
            float defaultY = canvas.yMin;
            unlockedDefs.Clear();
            List<Pair<Def, string>> unlocks = selected.Research.GetDirectUnlocks();
            var indirectUnlocks = selected.Research.GetIndirectUnlocks();
            indirectUnlocks.ForEach(x => unlocks.RemoveAll(y => y.First == x.First.First));

            float currentX = defaultX;
            float currentY = defaultY;
            foreach (Pair<Def, string> unlock in unlocks ?? Enumerable.Empty< Pair<Def, string>>())
            {
                Rect iconRect = new Rect(
                    currentX,
                    currentY,
                    iconSize.x,
                    iconSize.y);
                unlockedDefs.Add(new UnlockedDef(unlock.First, unlock.Second, iconRect));

                currentX += (iconSpace);
                if (currentX > canvas.xMax)
                {
                    currentX = defaultX;
                    currentY += iconSpace - 2f;
                }
            }

            foreach (Pair<Pair<ThingDef, string>, List<Pair<RecipeDef, string>>> item in indirectUnlocks ?? Enumerable.Empty< Pair<Pair<ThingDef, string>, List<Pair<RecipeDef, string>>>>())
            {
                float width = (item.Second.Count + 1) * (iconSpace);
                float newX = currentX + width + Margin;
                if (newX > canvas.xMax)
                {
                    newX = defaultX + width + Margin;
                    currentX = defaultX;
                    currentY += iconSize.y + 2f;
                }
                Rect iconRect = new Rect(
                    currentX,
                    currentY,
                    bigIconSize.x,
                    bigIconSize.y);
                Rect groupUnlockRect = new Rect(currentX, currentY, width, iconSize.y);
                unlockedDefs.Add(new UnlockedGroup(item.First.First, item.First.Second, iconRect, groupUnlockRect, item.Second));

                currentX = newX;
                
            }
            unlockedInit = true;
        }
    }
   
}