using DRimEditor.DetailView;
using DRimEditor.Research;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DRimEditor.Windows
{

    public class MainWindow : Window
    {
        public static MainWindow instance;

        public static TabWindow openWindow = null;

        public static Color lightGrey = new Color(0.8f, 0.8f, 0.8f);

        public static ResearchWindow researchWindow;
        public static ProfileManagerWindow profileManagerWindow = new ProfileManagerWindow();
        public static DefExplorerWindow defExplorerWindow;

        public static Texture2D backTex;
        public static Texture2D backTexGrey;
        public static Texture2D forwardTex;
        public static Texture2D forwardTexGrey;

        public static bool initialized = false;

        public Rect viewRect;
        public Rect topBarRect;
        public Rect bottomBarRect;

        public static Stack<Action> backStack = new Stack<Action>();
        public static Stack<Action> forwardStack = new Stack<Action>();

        public const float TopBarHeight = 50;
        public const float BottomBarHeight = 58;

        public static Action curCommand;

        public MainWindow()
        {
            closeOnClickedOutside = false;
            instance = this;
        }

        public override void PreClose()
        {
            base.PreClose();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            closeOnClickedOutside = false;

            SetRects();

            if (!initialized)
            {
                researchWindow = new ResearchWindow();
                defExplorerWindow = new DefExplorerWindow();
                openWindow = profileManagerWindow;
                backTex = ContentFinder<Texture2D>.Get("Buttons/rimEditBack");
                backTexGrey = ContentFinder<Texture2D>.Get("Buttons/rimEditBackGrey");
                forwardTex = ContentFinder<Texture2D>.Get("Buttons/rimEditForward");
                forwardTexGrey = ContentFinder<Texture2D>.Get("Buttons/rimEditForwardGrey");
                initialized = true;
            }
        }

        private void SetRects()
        {
            windowRect.x = 0f;
            windowRect.y = 0f;
            windowRect.width = UI.screenWidth;
            windowRect.height = UI.screenHeight;

            viewRect = new Rect(
                 Constants.Margin,
                TopBarHeight + Margin,
                (windowRect.width - 12f),
                (windowRect.height - TopBarHeight - BottomBarHeight - StandardMargin*2));

            topBarRect = new Rect(
                0f,
                0f,
                windowRect.width,
                TopBarHeight);

            bottomBarRect = new Rect(
                windowRect.xMin,
                windowRect.yMax - BottomBarHeight - StandardMargin,
                windowRect.width,
                BottomBarHeight);

            profileManagerWindow.Init(viewRect);
        }

        public static void Back()
        {
            if (backStack.Count <= 0)
            {
                Verse.Log.Warning("RimEditor: Tried to go back with no window to go back to");
                return;
            }
            if (curCommand != null)
            {
                forwardStack.Push(curCommand);
            }
            curCommand = backStack.Pop();
            curCommand.Invoke();
        }
        
        public static void Forward()
        {
            if (forwardStack.Count <= 0)
            {
                Verse.Log.Warning("RimEditor: Tried to go forward with no window to go forward to");
                return;
            }
            if (curCommand != null)
            {
                backStack.Push(curCommand);
            }
            curCommand = forwardStack.Pop();
            curCommand.Invoke();
        }

        public static void Open(TabWindow window, bool clearForward = false)
        {
            if (openWindow != window)
            {
                if (openWindow != null)
                {
                    TabWindow cur = openWindow;
                    backStack.Push(delegate () { openWindow = cur; openWindow.PreActive(); });
                    curCommand = delegate () { openWindow = window; openWindow.PreActive(); };
                }
                if (clearForward)
                    forwardStack.Clear();

                if (window is ResearchWindow)
                {
                    if (!ResearchWindow.initialized)
                    {
                        Find.WindowStack.TryRemove(instance);
                        LongEventHandler.QueueLongEvent(DRimEditor.Research.Tree.Initialize, "DRimEditor.Research.BuildingResearchTree", false, null);
                        LongEventHandler.QueueLongEvent(() => openWindow = researchWindow, "", false, null);
                        return;
                    }
                }
                if (window is DefExplorerWindow)
                {
                    if (!DefExplorerWindow.initialized)
                    {
                        Find.WindowStack.TryRemove(instance);
                        LongEventHandler.QueueLongEvent(DatabaseBuilder.ResolveImpliedDefs, "BuildingRimEditDatabase", false, null);
                        LongEventHandler.QueueLongEvent(() => openWindow = defExplorerWindow, "", false, null);
                        return;
                    }
                }

                openWindow = window;
                openWindow.PreActive();
                
            }     
        }

        public override void OnAcceptKeyPressed()
        {
            return;
 
        }

        public override void DoWindowContents(Rect inRect)
        {
            //Rect researchButtonRect = topButtonsRect.LeftPartPixels(50);
            bool profileSet = ProfileManager.currentProfile != null;
            // Top Bar --------------------------------------------------------------------
            Rect backRect = topBarRect.LeftPartPixels(Text.LineHeight + 13f);
            backRect.height = Text.LineHeight + 13f;
            if (backStack.Count > 0)
            {
                bool back = Widgets.ButtonImage(backRect, backTex, baseColor: lightGrey, mouseoverColor: Color.white , doMouseoverSound: true);
                if (back)
                {
                    Back();
                }
            }
            else
            {
                Widgets.DrawTextureFitted(backRect, backTexGrey, 1f);
            }

            GUI.color = Color.white;

            Rect forwardRect = new Rect(backRect.xMax + 4f, backRect.y, backRect.width, backRect.height);
            if (forwardStack.Count > 0)
            {
                bool forward = Widgets.ButtonImage(forwardRect, forwardTex, baseColor: lightGrey, mouseoverColor: Color.white, doMouseoverSound: true);
                if (forward)
                {
                    Forward();
                }
            }
            else
            {
                Widgets.DrawTextureFitted(forwardRect, forwardTexGrey, 1f);
            }

            GUI.color = Color.white;

            Rect profileManagerRect = new Rect(forwardRect.xMax + 4f, forwardRect.y, 125f, forwardRect.height);
            bool profileManagerClicked = Widgets.ButtonText(profileManagerRect, "Profiles");

            Rect researchRect = new Rect(profileManagerRect.xMax + 4f, profileManagerRect.y, profileManagerRect.width, profileManagerRect.height);
            bool researchClicked = Widgets.ButtonText(researchRect, "Research", textColor: profileSet ? Color.white : Color.grey, drawBackground: profileSet, doMouseoverSound: profileSet, active: profileSet);

            Rect defExplorerRect = new Rect(researchRect.xMax + 4f, profileManagerRect.y, profileManagerRect.width, profileManagerRect.height);
            bool explorerClicked = Widgets.ButtonText(defExplorerRect, "Def Explorer", textColor: profileSet ? Color.white : Color.grey, drawBackground: profileSet, doMouseoverSound: profileSet, active: profileSet);

            

            float topRightBarx = bottomBarRect.xMax - 100f - Margin;
            if (openWindow == researchWindow)
            {
                Rect refreshRect = new Rect(topRightBarx, profileManagerRect.y, 75f, Text.LineHeight + 13f);
                bool refresh = Widgets.ButtonText(refreshRect, "Refresh");
                topRightBarx -= refreshRect.width + 5f;
                if (refresh)
                {
                    ResearchWindow.Refresh();
                    return;
                }
            }

            // Bottom Bar --------------------------------------------------------------------
            float buttonHeight = Text.LineHeight + 13f;
            Rect saveProfileRect = bottomBarRect.LeftPartPixels(100f);
            saveProfileRect.yMin = bottomBarRect.yMax - 50f;
            saveProfileRect.height = buttonHeight;
            bool saveProfile = Widgets.ButtonText(saveProfileRect, "Save Profile", textColor: profileSet ? Color.white : Color.grey, drawBackground: profileSet, doMouseoverSound: profileSet, active: profileSet);

            Rect unsavedChangesRect = new Rect(saveProfileRect.xMax + 4f, saveProfileRect.y + (saveProfileRect.height/3.0f), 150f, saveProfileRect.height);
            string unsavedText = "";
            if (ProfileManager.unsavedChanges)
                unsavedText = "(unsaved changes)";
            Widgets.Label(unsavedChangesRect, unsavedText);

            Rect closeRect = new Rect(bottomBarRect.xMax - 100f - Margin, saveProfileRect.yMin, 60f, Text.LineHeight+13f);
            bool close = Widgets.ButtonText(closeRect, "Close");
            // Clicks --------------------------------------------------------------------
  
                      
            if (profileManagerClicked || !profileSet)
            {
                
                Open(profileManagerWindow, true);
            }
            else if (researchClicked)
            {
                Open(researchWindow, true);
            }
            else if (explorerClicked)
            {
                Open(defExplorerWindow, true);
            }

            if (saveProfile && profileSet)
            {
                ProfileManager.SaveChanges();
            }
            
            if (close)
            {
                FileManager.SaveConfig();
                Find.WindowStack.TryRemove(typeof(MainWindow));
            }

            if (openWindow != null)
            {
                openWindow.DoWindowContents(viewRect);
            }

            
        }
    }
    public enum KnownWindows
    {
        research,
        allDefs
    }

}
