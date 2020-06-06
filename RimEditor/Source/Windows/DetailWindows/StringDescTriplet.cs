﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
    public class StringDescTriplet
    {
        public string StringDesc;
        public string Prefix;
        public string Suffix;

        private float _height;
        private bool _heightSet;

        public StringDescTriplet(string stringDesc, string prefix = null, string suffix = null)
        {
            StringDesc = stringDesc;
            Prefix = prefix;
            Suffix = suffix;
            _height = 0f;
            _heightSet = false;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            if (Prefix != "")
            {
                s.Append(Prefix + " ");
            }
            s.Append(StringDesc);
            if (Suffix != "")
            {
                s.Append(" " + Suffix);
            }
            return s.ToString();
        }

        public void Draw(ref Vector3 cur, Vector3 colWidths)
        {
            if (!_heightSet)
            {
                List<float> heights = new List<float>();
                if (!Prefix.NullOrEmpty())
                {
                    heights.Add(Text.CalcHeight(Prefix, colWidths.x));
                }
                heights.Add(Text.CalcHeight(StringDesc, colWidths.y));
                if (!Suffix.NullOrEmpty())
                {
                    heights.Add(Text.CalcHeight(Suffix, colWidths.z));
                }
                _height = heights.Max();
                _heightSet = true;
            }

            if (!Prefix.NullOrEmpty())
            {
                Rect prefixRect = new Rect(cur.x, cur.y, colWidths.x, _height);
                Widgets.Label(prefixRect, Prefix);
            }

            if (!Suffix.NullOrEmpty())
            {
                Rect suffixRect = new Rect(cur.x + colWidths.x + colWidths.y + 2 * DetailSection._columnMargin, cur.y, colWidths.z, _height);
                Widgets.Label(suffixRect, Suffix);
            }

            Rect labelRect =
                new Rect(cur.x + colWidths.x + (Prefix.NullOrEmpty() ? 0f : DetailSection._columnMargin),
                          cur.y,
                          colWidths.y,
                          _height);

            Widgets.Label(labelRect, StringDesc);
            cur.y += _height - DefExplorerWindow.LineHeightOffset;
        }
    }
}