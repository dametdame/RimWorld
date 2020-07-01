using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace DRimEditor.Windows.Dialogs
{
    public class SimpleTextEntryDialog : Window
    {
        protected string curEntry;
        protected string label = "";

        private float Height
        {
            get
            {
                return 200f;
            }
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(640f, this.Height);
            }
        }

        public SimpleTextEntryDialog(string title = "")
        {
            label = title;
            this.closeOnAccept = true;
            this.closeOnCancel = true;
            this.absorbInputAroundWindow = true;
        }

        public override void OnAcceptKeyPressed()
        {
            if (this.IsValid(this.curEntry))
            {
                this.DoAccept(this.curEntry);
                Verse.Find.WindowStack.TryRemove(this, true);
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;
            bool okKeyPressed = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                okKeyPressed = true;
                Event.current.Use();
            }

            Widgets.Label(new Rect(0f, 0f, rect.width, rect.height), label);
            this.curEntry = Widgets.TextField(new Rect(0f, 80f, rect.width / 2f + 70f, 35f), this.curEntry);

            Rect okRect = new Rect(rect.width / 2f + 90f, rect.height - 35f, rect.width / 2f - 90f, 35f);
            if (Widgets.ButtonText(okRect, "OK".Translate(), true, true, true) || okKeyPressed)
            {
                OnAcceptKeyPressed();
                Event.current.Use();
            }
        }

        protected virtual bool IsValid(string s)
        {
            return true;
        }

        protected virtual void DoAccept(string s)
        { }
    }
}
