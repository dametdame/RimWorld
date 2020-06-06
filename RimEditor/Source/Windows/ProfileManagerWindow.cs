using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Reflection.Emit;

namespace DRimEditor.Windows
{
    public class ProfileManagerWindow : TabWindow
    {
        public override Vector2 InitialSize => new Vector2(400f, 600f);

        public static Vector2 fileScrollPosition;
        public static Vector2 detailScrollPosition;

        public static Profile selectedProfile;

        public static string currentAddText = "";

        bool onlyShowErrored = true;

        public override void PreActive()
        {
            return;
        }

        public void Init(Rect canvas)
        {
            base.PreOpen();

            SetRects(canvas);

            closeOnClickedOutside = false;
        }

        private void SetRects(Rect canvas)
        {
            windowRect.x = canvas.x;
            windowRect.y = canvas.y;
            windowRect.width = canvas.width;
            windowRect.height = canvas.height;
        }

        public void DoSideButtons(Rect canvas)
        {
            float leftButtonsHeight = Text.LineHeight;

            bool haveSelected = selectedProfile != null;
            bool haveCurrent = ProfileManager.currentProfile != null;

            Rect addButtonRect = new Rect(canvas.x, canvas.y, canvas.width, leftButtonsHeight);
            bool addNew = Widgets.ButtonText(addButtonRect, "New");

            //Rect setButtonRect = new Rect(canvas.x, addButtonRect.yMax + Margin, canvas.width, leftButtonsHeight);
            //bool set = Widgets.ButtonText(setButtonRect, "Open", drawBackground: haveSelected, doMouseoverSound: haveSelected, textColor: haveSelected ? Color.white : Color.grey, active: haveSelected);

            Rect unsetButtonRect = new Rect(canvas.x, addButtonRect.yMax + Margin, canvas.width, leftButtonsHeight);
            bool unset = Widgets.ButtonText(unsetButtonRect, "Close", drawBackground: haveCurrent, doMouseoverSound: haveCurrent, textColor: haveCurrent ? Color.white : Color.grey, active: haveCurrent);

            Rect deleteButtonRect = new Rect(canvas.x, unsetButtonRect.yMax + Margin, canvas.width, leftButtonsHeight);
            bool delete = Widgets.ButtonText(deleteButtonRect, "Delete", drawBackground: haveSelected, doMouseoverSound: haveSelected, textColor: haveSelected ? Color.white : Color.grey, active: haveSelected);


            if (addNew)
            {
                if(!ProfileManager.profiles.Any(x => x.ToString() == currentAddText))
                {
                    ProfileManager.MakeNewProfile(currentAddText);
                }
            }
            else if (delete)
            {
                if (selectedProfile != null)
                {
                    ProfileManager.DeleteProfile(selectedProfile);
                }
            }
            /*else if (set)
            {
                if (selectedProfile != null)
                {
                    ProfileManager.SetProfile(selectedProfile);
                }
            }*/
            else if (unset)
            {
                if (ProfileManager.currentProfile != null)
                {
                    ProfileManager.UnsetProfile();
                }
            }

        }

        public void DoMainList(Rect canvas, float itemHeight)
        {
            int profileCount = ProfileManager.profiles.Count();
            float viewHeight = (float)profileCount * itemHeight;  

            Rect topLabel = new Rect(canvas.x, canvas.y, canvas.width, itemHeight);
            Widgets.Label(topLabel, "Profiles:");

            Rect addNameRect = new Rect(topLabel.x, topLabel.yMax, canvas.width, Text.LineHeight);
            currentAddText = Widgets.TextField(addNameRect, currentAddText);

            Rect outRect = new Rect(addNameRect.x, addNameRect.yMax + 5f, addNameRect.width, canvas.height);
            Rect viewRect = new Rect(addNameRect.x, addNameRect.yMax + 5f, addNameRect.width - 16f, viewHeight);

            float curHeight = 0f;
            Widgets.BeginScrollView(outRect, ref fileScrollPosition, viewRect, true);
            using (var enumerator = ProfileManager.profiles.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Profile current = enumerator.Current;
                    Rect rowRect = new Rect(outRect.x, outRect.y + curHeight, canvas.width, itemHeight);
                    if (selectedProfile == current)
                    {
                        Widgets.DrawHighlightSelected(rowRect);
                    }
                    else if (Mouse.IsOver(rowRect))
                    {
                        Widgets.DrawHighlight(rowRect);
                    }
                    else
                    {
                        Widgets.DrawLightHighlight(rowRect);
                    }
                    GUI.BeginGroup(rowRect);
                    Rect labelRect = new Rect(0f, 0f, canvas.width, itemHeight);
                    Widgets.Label(labelRect, current.ToString());
                    GUI.EndGroup();
                    if (Mouse.IsOver(rowRect) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        SelectProfile(current);
                    }
                    curHeight += itemHeight;
                }
            }
            Widgets.EndScrollView();

        }

        public static void SelectProfile(Profile profile)
        {
            selectedProfile = profile;
            currentAddText = profile.ToString();
            ProfileManager.currentProfile = profile;
        }

        public void DoRightPanel(Rect canvas)
        {
            Rect labelRect = new Rect(canvas.x, canvas.y, canvas.width, Text.LineHeight);
            Widgets.Label(labelRect, "Applied profile: " + (ProfileManager.activeProfile?.ToString() ?? "-"));
            Rect nextApplyRect = new Rect(canvas.x, labelRect.yMax + 4f, canvas.width, Text.LineHeight);
            Widgets.Label(nextApplyRect, "Profile to be applied next startup: " + (ProfileManager.nextActiveProfile?.ToString() ?? "-"));
            Rect examineRect = new Rect(canvas.x, nextApplyRect.yMax + 4f, canvas.width, Text.LineHeight);
            Widgets.Label(examineRect, "Currently editing: " + (ProfileManager.currentProfile?.ToString() ?? "-") + (ProfileManager.unsavedChanges ? " (unsaved changes)" : ""));

            bool canApplyNextProfile = ProfileManager.currentProfile != null && ProfileManager.currentProfile != ProfileManager.nextActiveProfile;
            bool canUnapplyNextProfile = ProfileManager.nextActiveProfile != null;

            Rect applyRect = new Rect(canvas.x, examineRect.yMax + 4f, 200f, Text.LineHeight);
            bool applyClicked = Widgets.ButtonText(applyRect, "Apply Current", drawBackground: canApplyNextProfile, doMouseoverSound: canApplyNextProfile, textColor: canApplyNextProfile ? Color.white : Color.grey, active: canApplyNextProfile);
            Rect unapplyRect = new Rect(applyRect.xMax + Margin, examineRect.yMax + 4f, 200f, Text.LineHeight);
            bool unapplyClicked = Widgets.ButtonText(unapplyRect, "Unapply Current", drawBackground: canUnapplyNextProfile, doMouseoverSound: canUnapplyNextProfile, textColor: canUnapplyNextProfile ? Color.white : Color.grey, active: canUnapplyNextProfile);
            Rect onlyErroredRect = new Rect(unapplyRect);
            onlyErroredRect.x = unapplyRect.xMax + Margin;
            
            Widgets.CheckboxLabeled(onlyErroredRect, "Only show errors", ref onlyShowErrored);

            if (applyClicked && canApplyNextProfile)
            {
                ProfileManager.SetProfileToBeApplied();
                Dialog_MessageBox inform = new Dialog_MessageBox("Profile " + ProfileManager.nextActiveProfile + " will be applied after restart.");
                Find.WindowStack.Add(inform);
            }
            else if (unapplyClicked && canUnapplyNextProfile)
            {
                if (ProfileManager.activeProfile != null && ProfileManager.activeProfile == ProfileManager.nextActiveProfile)
                { 
                    Dialog_MessageBox inform = new Dialog_MessageBox("Profile will be unapplied after restart.");
                    Find.WindowStack.Add(inform);
                }
                ProfileManager.UnsetProfileToBeApplied();  
            }

            
            float totalTopHeight =  labelRect.height + nextApplyRect.height + examineRect.height +  applyRect.height + unapplyRect.height + 4f;
            float viewRectHeight = canvas.height - totalTopHeight;
            if (ProfileManager.currentProfile != null)
            {
                viewRectHeight = ProfileManager.currentProfile.getLoaded.Count() * Text.LineHeight * 2f;
            }
            Rect outRect = new Rect(canvas.x, unapplyRect.yMax + 4f, canvas.width, canvas.height - totalTopHeight);
            Rect viewRect = new Rect(canvas.x, unapplyRect.yMax + 4f, canvas.width - 16f, viewRectHeight);
            Widgets.DrawBoxSolid(outRect, Color.black);

            float curHeight = 0f;
            float curY = viewRect.yMin;
            float oldY = curY;
            int index = 0;
            Widgets.BeginScrollView(outRect, ref detailScrollPosition, viewRect, true);
            using (List<string>.Enumerator? enumerator = ProfileManager.currentProfile?.getLoaded.GetEnumerator())
            {
                if (enumerator is List<string>.Enumerator iterate)
                {
                    bool noErrors = onlyShowErrored;
                    while (iterate.MoveNext())
                    {
                        string current = iterate.Current;
                        if (onlyShowErrored && !ProfileManager.currentProfile.HasError(current))
                        {
                            index++;
                            continue;
                        }
                        Widgets.LongLabel(outRect.x, outRect.width, Profile.FormatComand(current), ref curY);
                        float itemHeight = curY - oldY;
                        Rect rowRect = new Rect(outRect.x, outRect.y + curHeight, canvas.width, itemHeight);
                        if (ProfileManager.currentProfile.HasError(current))
                        {
                            GUI.color = Color.yellow;
                            Widgets.DrawHighlightSelected(rowRect);
                        }
                        if (Mouse.IsOver(rowRect))
                        {
                            Widgets.DrawHighlight(rowRect);
                        }
                        GUI.color = Color.white;
                        if (Mouse.IsOver(rowRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1)
                        {
                            int curIndex = index;
                            List<FloatMenuOption> options = new List<FloatMenuOption> { new FloatMenuOption("Delete command", () => ProfileManager.DeleteCommand(curIndex), MenuOptionPriority.Default, null) };
                            if (ProfileManager.currentProfile.HasError(current))
                            {
                                options.Add(new FloatMenuOption(ProfileManager.currentProfile.GetError(current), null));
                            }
                            Find.WindowStack.Add(new FloatMenu(options));
                            Event.current.Use();
                        }
                        curHeight += itemHeight;
                        oldY = curY;
                        index++;
                        noErrors = false;
                    }
                    if (ProfileManager.currentProfile?.getLoaded == null || ProfileManager.currentProfile.getLoaded.Count == 0)
                    {
                        Rect rowRect = new Rect(outRect.x, outRect.y + curHeight, canvas.width, Text.LineHeight);
                        Widgets.Label(rowRect, "Empty profile");
                    }
                    else if (noErrors)
                    {
                        Rect rowRect = new Rect(outRect.x, outRect.y + curHeight, canvas.width, Text.LineHeight);
                        Widgets.Label(rowRect, "No errors");
                    }
                    
                }
                else
                {
                    Rect rowRect = new Rect(outRect.x, outRect.y + curHeight, canvas.width, Text.LineHeight);
                    Widgets.Label(rowRect, "No profile loaded");
                }
            }
            Widgets.EndScrollView();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Escape))
            {
                UI.UnfocusCurrentControl();
            }

            float columnHeight = inRect.height;
            float columnWidth = inRect.width / 3f;
            float columnY = inRect.y;
            float itemHeight = 40f;

            Rect leftButtonRect = new Rect(inRect.x, columnY + itemHeight, 75f, columnHeight);
            DoSideButtons(leftButtonRect);

            float listXOffset = leftButtonRect.xMax + Margin;

            Rect listRect = new Rect(inRect.x + listXOffset, columnY, columnWidth, columnHeight);
            DoMainList(listRect, Text.LineHeight + 2f);

            Rect panelRect = new Rect(listRect.xMax + Margin, columnY, inRect.width - listRect.width - leftButtonRect.width - Margin*3, columnHeight);
            DoRightPanel(panelRect);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                UI.UnfocusCurrentControl();
            }
        }

    }
}
