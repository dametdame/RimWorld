using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static DRimEditor.Research.Assets;
using static DRimEditor.Research.Constants;
using DRimEditor.Research;
using DRimEditor.Windows;

namespace DRimEditor.ResearchPal.RPUtility
{
    public class UnlockedDef
    {
        public Def def;
        public Rect rect;
        public string description;
        public Rect dragRect;
        public bool canDrag;

        public bool clicked = false;
        

        public UnlockedDef(Def d, string s, Rect canvas, bool draggable = true)
        {
            def = d;
            description = s;
            rect = canvas;
            canDrag = draggable;
        }

        public virtual void Draw()
        {
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
                {
                    clicked = true;
                }
                else if (clicked && ResearchSelectPanel._instance.draggingDef == null)
                {
                    if (Event.current.type == EventType.MouseUp)
                    {
                        clicked = false;
                        ClickHandler();
                    }
                    else if (Event.current.type == EventType.MouseDrag && canDrag)
                    {
                        clicked = false;
                        ResearchSelectPanel._instance.draggingDef = this;
                        ResearchSelectPanel._instance._mousePosition = Event.current.mousePosition;
                        dragRect = new Rect(rect);
                        Event.current.Use();
                    }
                }
                else if (canDrag && ResearchSelectPanel._instance.draggingDef == this && Event.current.type == EventType.MouseDrag)
                {
                    var currentMousePosition = Event.current.mousePosition;
                    dragRect.x = currentMousePosition.x;
                    dragRect.y = currentMousePosition.y;
                    ResearchSelectPanel._instance._mousePosition = currentMousePosition;
                }
            }
            else if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption> { new FloatMenuOption("Remove Unlock", () => ResearchProjectEditor.RemoveUnlockable(ResearchSelectPanel.selected, this.def), MenuOptionPriority.Default, null) };
                    Find.WindowStack.Add(new FloatMenu(options));
                    Event.current.Use();
                }
            }
            if (canDrag && ResearchSelectPanel._instance.draggingDef == this)
            {
                def.DrawColouredIcon(dragRect);
            }
            else
            {
                def.DrawColouredIcon(rect);
                TooltipHandler.TipRegion(rect, description);
            }

            if (!canDrag)
            {
                Rect lockRect = new Rect(rect.xMax - 10f, rect.yMin, 10f, 10f);
                GUI.DrawTexture(lockRect, Assets.Lock, ScaleMode.ScaleToFit);
            }
        }

        public void ClickHandler()
        {
            MainWindow.defExplorerWindow.JumpTo(this.def);
        }
    }

    public class UnlockedGroup : UnlockedDef
    {
        List<UnlockedDef> recipes = new List<UnlockedDef>();
        //public List<Pair<RecipeDef, string>> recipes;
        public Rect gr;
        bool bigIcons = true;

        public UnlockedGroup(Def d, string s, Rect benchRect, Rect groupRect, List<Pair<RecipeDef, string>> rl, bool useBigIcons = true) : base(d, s, benchRect)
        {
            def = d;
            description = s;
            rect = benchRect;
            
            gr = groupRect;
            bigIcons = useBigIcons;
            float space = (bigIcons ? bigIconSize.x : IconSize.x) + 4f;
            float curX = benchRect.x;
            
            foreach(Pair<RecipeDef, string> item in rl)
            {
                curX += space;
                Rect iconRect = new Rect(benchRect);
                iconRect.x = curX;
                recipes.Add(new UnlockedDef(item.First, item.Second, iconRect, draggable: false));
            }
        }
        public override void Draw()
        {
            GUI.DrawTexture(gr, Assets.LightBackground);
            base.Draw();
            foreach(UnlockedDef def in recipes)
            {
                def.Draw();
            }
        }
    }
}
