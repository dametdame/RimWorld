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
    public class Dialog_ListSelect : Window
    {
        List<string> options = new List<string>();
        Dictionary<string, bool> chosen = new Dictionary<string, bool>();

        private float Height
        {
            get
            {
                return 100f + options.Count * (Text.LineHeight + 5f);
            }
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(640f, this.Height);
            }
        }

        public Dialog_ListSelect(List<string> options)
        {
            if (options == null || options.Count == 0)
            {
                Verse.Find.WindowStack.TryRemove(this, true);
                return;
            }
            this.options = options;
            foreach (string option in options)
                chosen.Add(option, false);
            chosen[options[0]] = true;
            this.closeOnAccept = true;
            this.closeOnCancel = true;
            this.absorbInputAroundWindow = true;
        }

        public override void OnAcceptKeyPressed()
        {
            foreach (string option in options)
            {
                if (chosen[option])
                {
                    DoAccept(option);
                    break;
                }
            }
            Verse.Find.WindowStack.TryRemove(this, true);
        }

        public virtual void DoAccept(string option) { }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;

            float curY = rect.yMin;
            foreach (string option in options)
            {
                Rect optionRect = new Rect(rect.xMin, curY, rect.width, Text.LineHeight);
                bool newChosen = Widgets.RadioButtonLabeled(optionRect, option, chosen[option]);
                if (newChosen)
                {
                    foreach (string otherOption in options)
                    {
                        if (otherOption != option)
                            chosen[otherOption] = false;
                    }
                    chosen[option] = true;
                }
                curY += Text.LineHeight + 5f;
            }

            bool okKeyPressed = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                okKeyPressed = true;
                Event.current.Use();
            }
            Rect cancelRect = new Rect(rect.width / 2f - 45f, rect.height - 35f, 80f, 35f);
            Rect okRect = new Rect(rect.width / 2f + 45f, rect.height - 35f, 80f, 35f);
            if (Widgets.ButtonText(cancelRect, "Cancel", true, true, true))
            {
                Verse.Find.WindowStack.TryRemove(this, true);
                Event.current.Use();
            }
            if (Widgets.ButtonText(okRect, "OK".Translate(), true, true, true) || okKeyPressed)
            {
                OnAcceptKeyPressed();
            }
        }
    }
}
