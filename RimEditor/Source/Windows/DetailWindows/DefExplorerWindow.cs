using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using DRimEditor.DetailView;
using DRimEditor.Windows;
using System.Reflection;
using HarmonyLib;
using Verse.Noise;

namespace DRimEditor.DetailView
{

    public class DefExplorerWindow : TabWindow, DefView
    {

        protected static List<ModCategory> CachedHelpCategories;
        public ItemDetailDef SelectedItemDef;

        public const float WindowMargin = 6f; // 15 is way too much.
        public const float EntryHeight = 30f;
        public const float EntryIndent = 15f;
        public const float ParagraphMargin = 8f;
        public const float LineHeightOffset = 6f; // CalcSize overestimates required height by roughly this much.

        protected Rect SelectionRect;
        protected Rect DisplayRect;
        protected static Vector2 ArrowImageSize = new Vector2(10f, 10f);

        protected Vector2 SelectionScrollPos = default(Vector2);
        protected Vector2 DisplayScrollPos = default(Vector2);

        public const float MinWidth = 800f;
        public const float MinHeight = 600f;
        public const float MinListWidth = 400f;
        public float ContentHeight = 9999f;
        public float ContentWidth = 9999f;
        public float SelectionHeight = 9999f;

        public static bool initialized = false;

        private static string _filterString = "";
        private string _lastFilterString = "";
        private bool _filtered;
        private bool _jump;

        public static DefExplorerWindow instance;

        public static FieldDesc activeDesc;

        public static bool TryChangeActiveDesc(FieldDesc newDesc)
        {
            if (newDesc != activeDesc)
            {
                activeDesc?.Reset();
                activeDesc = newDesc;
                return true;
            }
            return false;
        }

        /*
        private MainButton_HelpMenuDef TabDef
        {
            get
            {
                return def as MainButton_HelpMenuDef;
            }
        }
        */

        public DefExplorerWindow()
        {
            //layer = WindowLayer.GameUI;
            soundAppear = null;
            soundClose = null;
            doCloseButton = false;
            doCloseX = true;
            closeOnCancel = true;
            forcePause = true;
            instance = this;
        }
        public class ModCategory
        {
            readonly List<CategoryDef> _helpCategories = new List<CategoryDef>();

            public readonly string ModName;

            public bool Expanded;

            public ModCategory(string modName)
            {
                ModName = modName;
            }

            public List<CategoryDef> HelpCategories
            {
                get
                {
                    return _helpCategories.OrderBy(a => a.label).ToList();
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
                    (ModName.ToUpper().Contains(filter.ToUpper()))
                );
            }

            public bool ThisOrAnyChildMatchesFilter(string filter)
            {
                return (
                    (MatchesFilter(filter)) ||
                    (HelpCategories.Any(hc => hc.ThisOrAnyChildMatchesFilter(filter)))
                );
            }

            public void Filter(string filter)
            {
                ShouldDraw = ThisOrAnyChildMatchesFilter(filter);
                Expanded = (
                    (filter != "") &&
                    (ShouldDraw)
                );

                foreach (CategoryDef hc in HelpCategories)
                {
                    hc.Filter(filter, MatchesFilter(filter));
                }
                /*
                ShouldDraw = ThisOrAnyChildMatchesFilter(filter);
                Expanded = (
                    (filter != "") &&
                    (ThisOrAnyChildMatchesFilter(filter))
                );

                foreach (CategoryDef hc in HelpCategories)
                {
                    hc.Filter(filter, MatchesFilter(filter));
                }*/
            }

            public void AddCategory(CategoryDef def)
            {
                _helpCategories.AddUnique(def);
            }
        }


        public override void PreOpen()
        {
            base.PreOpen();
            if (!initialized)
            {
                // Build the help system
                Recache();

                // set initial Filter
                Filter();
                initialized = true;
            }
        }

        public override void PreActive()
        {
            
            return;
        }

        public static void Recache()
        {
            CachedHelpCategories = new List<ModCategory>();
            foreach (var helpCategory in DefDatabase<CategoryDef>.AllDefs)
            {
                // parent modcategory does not exist, create it.
                if (CachedHelpCategories.All(t => t.ModName != helpCategory.ModName))
                {
                    var mCat = new ModCategory(helpCategory.ModName);
                    mCat.AddCategory(helpCategory);
                    CachedHelpCategories.Add(mCat);
                }
                // add to existing modcategory
                else
                {
                    var mCat = CachedHelpCategories.Find(t => t.ModName == helpCategory.ModName);
                    mCat.AddCategory(helpCategory);
                }
            }
        }
        private void _filterUpdate()
        {
            // filter after a short delay.
            if (_filterString != _lastFilterString)
            {
                _lastFilterString = _filterString;
                //_lastFilterTick = 0;
                _filtered = false;
            }
            else if (!_filtered)
            {
                //if (_lastFilterTick > 60)
                //{
                    Filter();
                //}
                //_lastFilterTick++;
            }
        }

        public void Filter()
        {
            foreach (ModCategory mc in CachedHelpCategories)
            {
                mc.Filter(_filterString);
            }
            _filtered = true;
        }

        public void ResetFilter()
        {
            _filterString = "";
            _lastFilterString = "";
            Filter();
        }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;

            GUI.BeginGroup(rect);

            //float selectionWidth = TabDef != null ? (TabDef.listWidth >= MinListWidth ? TabDef.listWidth : MinListWidth) : MinListWidth;
            float selectionWidth = MinListWidth;
            SelectionRect = new Rect(0f, 0f, selectionWidth, rect.height);
            DisplayRect = new Rect(
                SelectionRect.width + WindowMargin, 0f,
                rect.width - SelectionRect.width - WindowMargin - 30f, rect.height
            );

            DrawSelectionArea(SelectionRect);
            DrawDisplayArea(DisplayRect);

            GUI.EndGroup();

            if ((Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                || (Event.current.type == EventType.MouseDown && Event.current.button == 0))
            {
                UI.UnfocusCurrentControl();
                TryChangeActiveDesc(null);
            }
        }

        void DrawDisplayArea(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            if (SelectedItemDef == null)
            {
                return;
            }

            Text.Font = GameFont.Medium;
            Text.WordWrap = false;
            float titleWidth = Text.CalcSize(SelectedItemDef.LabelCap).x;
            var titleRect = new Rect(rect.xMin + WindowMargin, rect.yMin + WindowMargin, titleWidth, 60f);

           
            
              if ((SelectedItemDef.keyDef != null) && (SelectedItemDef.keyDef.IconTexture() != null))
            {
                var iconRect = new Rect(titleRect.xMin + WindowMargin, rect.yMin + WindowMargin, 60f - 2 * WindowMargin, 60f - 2 * WindowMargin);
                titleRect.x += 60f;
                SelectedItemDef.keyDef.DrawColouredIcon(iconRect);
            }

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(titleRect, SelectedItemDef.LabelCap);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;

            Rect outRect = rect.ContractedBy(WindowMargin);
            outRect.yMin += 60f;
            Rect viewRect = outRect;
           
            viewRect.width = ContentWidth - 16f;
            viewRect.height = ContentHeight;

            GUI.BeginGroup(outRect);
            Widgets.BeginScrollView(outRect.AtZero(), ref DisplayScrollPos, viewRect.AtZero());

            Vector3 cur = Vector3.zero;

            DetailSectionHelper.DrawText(ref cur, viewRect.width, SelectedItemDef.description);

            cur.y += ParagraphMargin;

            foreach (DetailWrapper section in SelectedItemDef.HelpDetailWrappers)
            {
                section.Draw(ref cur, outRect.width, this);
            }

            ContentHeight = cur.y;
            ContentWidth = cur.z + 20f;
            if (ContentWidth < outRect.width)
                ContentWidth = outRect.width;

            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        void DrawSelectionArea(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            _filterUpdate();
            Rect filterRect = new Rect(rect.xMin + WindowMargin, rect.yMin + WindowMargin, rect.width - 3 * WindowMargin - 30f, 30f);
            Rect clearRect = new Rect(filterRect.xMax + WindowMargin + 3f, rect.yMin + WindowMargin + 3f, 24f, 24f);
            _filterString = Widgets.TextField(filterRect, _filterString);
            if (_filterString != "")
            {
                if (Widgets.ButtonImage(clearRect, Widgets.CheckboxOffTex))
                {
                    _filterString = "";
                    Filter();
                }
            }

            Rect outRect = rect;
            outRect.yMin += 40f;
            outRect.xMax -= 2f; // some spacing around the scrollbar

            float viewWidth = SelectionHeight > outRect.height ? outRect.width - 16f : outRect.width;
            var viewRect = new Rect(0f, 0f, viewWidth, SelectionHeight);

            GUI.BeginGroup(outRect);
            Widgets.BeginScrollView(outRect.AtZero(), ref SelectionScrollPos, viewRect);

            if (CachedHelpCategories.Count(mc => mc.ShouldDraw) < 1)
            {
                Rect messageRect = outRect.AtZero();
                Widgets.Label(messageRect, "NoRimEditDefs".Translate());
            }
            else
            {
                Vector2 cur = Vector2.zero;

                // This works fine for the current artificial three levels of helpdefs. 
                // Can easily be adapted by giving each category a list of subcategories, 
                // and migrating the responsibility for drawing them and the helpdefs to DrawCatEntry().
                // Would also require some minor adaptations to the filter methods, but nothing major.
                // - Fluffy.
                float leeway = EntryHeight + 10f;
                foreach (ModCategory mc in CachedHelpCategories.Where(mc => mc.ShouldDraw))
                {
                    DrawModEntry(ref cur, 0, viewRect, mc);

                    cur.x += EntryIndent;
                    if (mc.Expanded)
                    {
                        foreach (CategoryDef hc in mc.HelpCategories.Where(hc => hc.ShouldDraw))
                        {

                            DrawCatEntry(ref cur, 1, viewRect, hc);

                            if (hc.Expanded)
                            {
                                foreach (ItemDetailDef idd in hc.ItemDetailDefs.Where(hd => hd.ShouldDraw))
                                {
                                    if ((SelectedItemDef == idd && _jump) || (cur.y + leeway >= SelectionScrollPos.y && cur.y <= SelectionScrollPos.y + outRect.height + 10f))
                                    {
                                        DrawItemEntry(ref cur, 1, viewRect, idd);
                                    }
                                    else
                                    {
                                        cur.y += EntryHeight;
                                    }
                                }
                            }
                            
                        }
                    }

                }

                SelectionHeight = cur.y;
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        public enum State
        {
            Expanded,
            Closed,
            Leaf
        }

        public void DeleteDef(ItemDetailDef def)
        {
            ParseHelper.RemoveFromDefDatabase(def.keyDef);
            string command = "".Remove().Find(def.keyDef);
            ProfileManager.AddCommand(command);
            if (SelectedItemDef == def)
            {
                SelectedItemDef = null;
            }
            def.category.ItemDetailDefs.Remove(def);
            ParseHelper.RemoveFromDefDatabase(def);
            //Recache();
        }

        public void AddDef(CategoryDef cat)
        {
            Def newDef = ParseHelper.MakeTypedObject(cat.defType) as Def;
            string newName = "!`newDefPleaseChange`!";
            newDef.defName = newName;
            newDef.label = newName;
            newDef.description = newName;
            ParseHelper.AddToDefDatabase(newDef);
            ProfileManager.AddCommand("".Push().New(cat.defType));
            ProfileManager.AddCommand("".Push().Pop().Set(new { newDef.defName }).Find(newDef.defName));
            ProfileManager.AddCommand("".Push().Pop().Set(new { newDef.label }).Find(newDef.label));
            ProfileManager.AddCommand("".Push().Pop().Set(new { newDef.description }).Find(newDef.description));
            ProfileManager.AddCommand("".Add().Pop());
            ItemDetailDef newItem = DatabaseBuilder.HelpForDef(newDef, cat);
            cat.Add(newItem);
            newItem.Filter(_filterString);
            DefDatabase<ItemDetailDef>.Add(newItem);
            //cat.Recache();
        }

        /// <summary>
        /// Generic method for drawing the squares. 
        /// </summary>
        /// <param name="cur">Current x,y vector</param>
        /// <param name="nestLevel">Level of nesting for indentation</param>
        /// <param name="view">Size of viewing area (assumed vertically scrollable)</param>
        /// <param name="label">Label to show</param>
        /// <param name="state">State of collapsing icon to show</param>
        /// <param name="selected">For leaf entries, is this entry selected?</param>
        /// <returns></returns>
        public bool DrawEntry(ref Vector2 cur, int nestLevel, Rect view, string label, State state, bool selected = false, ItemDetailDef itemDetailDef = null, CategoryDef cat = null)
        {
            cur.x = nestLevel * EntryIndent;
            float iconOffset = ArrowImageSize.x + 2 * WindowMargin;
            float width = view.width - cur.x - iconOffset - WindowMargin;
            float height = EntryHeight;

            if (Text.CalcHeight(label, width) > EntryHeight)
            {
                Text.Font = GameFont.Tiny;
                float height2 = Text.CalcHeight(label, width);
                height = Mathf.Max(height, height2);
            }

            if (state != State.Leaf)
            {
                Rect iconRect = new Rect(cur.x + WindowMargin, cur.y + height / 2 - ArrowImageSize.y / 2, ArrowImageSize.x, ArrowImageSize.y);
                GUI.DrawTexture(iconRect, state == State.Expanded ? ResourceBank.Icon.HelpMenuArrowDown : ResourceBank.Icon.HelpMenuArrowRight);
            }

            Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = new Rect(cur.x + iconOffset, cur.y, width, height);
            Widgets.Label(labelRect, label);
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            // full viewRect width for overlay and button
            Rect buttonRect = view;
            buttonRect.yMin = cur.y;
            cur.y += height;
            buttonRect.yMax = cur.y;
            if (Mouse.IsOver(buttonRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                var options = new List<FloatMenuOption>();
                CategoryDef curCat = null; 
                if (itemDetailDef != null)
                {
                    options.Add(new FloatMenuOption("Delete this, defName: " + itemDetailDef.keyDef.defName, () => DeleteDef(itemDetailDef), MenuOptionPriority.Default, null));
                    curCat = itemDetailDef.category;
                }
                else if (cat != null)
                {
                    curCat = cat;
                }

                if (itemDetailDef != null || cat != null)
                {
                    options.Add(new FloatMenuOption("Add new def of type " + curCat.defType.ToString(), () => AddDef(curCat), MenuOptionPriority.High, null));
                }
                else
                {
                    options.Add(new FloatMenuOption("(nothing to do)", null, null));
                }

                Find.WindowStack.Add(new FloatMenu(options));
                Event.current.Use();
            }
            GUI.color = Color.grey;
            Widgets.DrawLineHorizontal(view.xMin, cur.y, view.width);
            GUI.color = Color.white;
            if (selected)
            {
                Widgets.DrawHighlightSelected(buttonRect);
            }
            else
            {
                Widgets.DrawHighlightIfMouseover(buttonRect);
            }
            return Widgets.ButtonInvisible(buttonRect);
        }

        public void DrawModEntry(ref Vector2 cur, int nestLevel, Rect view, ModCategory mc)
        {
            State curState = mc.Expanded ? State.Expanded : State.Closed;
            if (DrawEntry(ref cur, nestLevel, view, mc.ModName, curState))
            {
                mc.Expanded = !mc.Expanded;
            }
        }

        public void DrawCatEntry(ref Vector2 cur, int nestLevel, Rect view, CategoryDef catDef)
        {
            State curState = catDef.Expanded ? State.Expanded : State.Closed;
            if (DrawEntry(ref cur, nestLevel, view, catDef.LabelCap, curState, cat: catDef))
            {
                catDef.Expanded = !catDef.Expanded;
            }
        }

        public void DrawItemEntry(ref Vector2 cur, int nestLevel, Rect view, ItemDetailDef itemDef)
        {
            bool selected = SelectedItemDef == itemDef;
            if (selected && _jump)
            {
                SelectionScrollPos.y = cur.y;
                _jump = false;
            }
            if (DrawEntry(ref cur, nestLevel, view, itemDef.LabelCap, State.Leaf, selected, itemDetailDef: itemDef))
            {
                SelectedItemDef = itemDef;
            }
        }

        public void JumpTo(Def def)
        {
            if (def == null)
                return;
            MainWindow.Open(this);
            LongEventHandler.QueueLongEvent(() => DoJumpTo(def), "", false, null);
            MainWindow.curCommand = delegate () { JumpTo(def); };
        }

        public void DoJumpTo(Def def)
        {
            DoJumpTo(def.GetDef());
        }

        public void JumpTo(ItemDetailDef helpDef)
        {
            if (helpDef == null)
                return;
            MainWindow.Open(this);
            LongEventHandler.QueueLongEvent(() => DoJumpTo(helpDef), "", false, null);
        }

        public void DoJumpTo(ItemDetailDef helpDef)
        {
            ResetFilter();
            _jump = true;
            SelectedItemDef = helpDef;
            CategoryDef cat = DefDatabase<CategoryDef>.AllDefsListForReading.First(hc => hc.ItemDetailDefs.Contains(helpDef));
            cat.Expanded = true;
            ModCategory mod = CachedHelpCategories.First(mc => mc.HelpCategories.Contains(cat));
            mod.Expanded = true;
        }

        public bool Accept(ItemDetailDef def)
        {
            return true;
        }

        public DefView SecondaryView(ItemDetailDef def)
        {
            return null;
        }

    }

}