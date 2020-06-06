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

        public static ProfileManagerWindow profileManagerWindow = new ProfileManagerWindow();

        public static bool initialized = false;

        public Rect viewRect;
        public Rect bottomBarRect;

        public const float BottomBarHeight = 58;

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
                Open(profileManagerWindow);
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
                 8f,
                 8f,
                (windowRect.width - 12f),
                (windowRect.height - BottomBarHeight - StandardMargin*2));

            bottomBarRect = new Rect(
                windowRect.xMin,
                windowRect.yMax - BottomBarHeight - StandardMargin,
                windowRect.width,
                BottomBarHeight);

            profileManagerWindow.Init(viewRect);
        }

        public static void Open(TabWindow window)
        {
            if (openWindow != window)
            {
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
           
            GUI.color = Color.white;

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
