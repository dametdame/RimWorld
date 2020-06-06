using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using DRimEditor.DetailView;
using System.Reflection;


namespace DRimEditor.DetailView
{
	
	public class DetailSection
	{
		public string Label;
		public string Label2 = "Def";
		public string InsetString;
		public float Inset;
		private const string DefaultInsetString = "\t";
		private const float DefaultInset = 30f;
		public List<DefStringTriplet> KeyDefs;
		public List<StringDescTriplet> StringDescs;
		public List<FieldDesc> ParentFieldDescs;
		public List<FieldDesc> FieldDescs;
		public bool Align;
		public Vector3 ColumnWidths = Vector3.zero;
		public bool WidthsSet = false;
		public static float _columnMargin = 8f;

		public bool initialized = false;

		FieldInfo[] rawFields;

		public FieldDesc parentDesc = null;
		public DetailWrapper wrapper;

		public object parentObject;

		public DetailSection(	object parent,
								string label,
								 FieldInfo[] fields,
								 DetailWrapper parentWrapper,
								 bool align = true,
								 FieldDesc parentDesc = null)
		{
			if (fields == null)
			{
				Log.Warning("Building DetailSection with empty fields");
			}
			this.parentObject = parent;
			this.parentDesc = parentDesc;
			rawFields = fields;
			Label = label;
			KeyDefs = null;
			wrapper = parentWrapper;
			//if (parent is Def)
			//	ParentFieldDescs = !DatabaseBuilder.allDefFields.NullOrEmpty() ? DetailSectionHelper.BuildFieldDescList(parent, DatabaseBuilder.allDefFields.ToArray(), parentDesc) : null;
			Align = align;
			if (label != null)
			{
				InsetString = DefaultInsetString;
				Inset = DefaultInset;
			}
			else
			{
				InsetString = string.Empty;
				Inset = 0f;
			}
		}

		public DetailSection(string label,
								string[] stringDescs,
								string[] prefixes,
								string[] suffixes,
								bool align = true)
		{
			Label = label;
			KeyDefs = null;
			StringDescs = stringDescs != null ? DetailSectionHelper.BuildStringDescTripletList(stringDescs, prefixes, suffixes) : null;
			Align = align;
			if (label != null)
			{
				InsetString = DefaultInsetString;
				Inset = DefaultInset;
			}
			else
			{
				InsetString = string.Empty;
				Inset = 0f;
			}
		}

		/*
		public DetailSection(string label,
								  List<Def> keyDefs,
								  string[] defPrefixes,
								  string[] defSuffixes,
								  string[] stringDescs,
								  string[] descPrefixes,
								  string[] descSuffixes,
								  bool align = true)
		{
			Label = label;
			KeyDefs = keyDefs != null ? DetailSectionHelper.BuildDefStringTripletList(keyDefs, defPrefixes, defSuffixes) : null;
			StringDescs = stringDescs != null ? DetailSectionHelper.BuildStringDescTripletList(stringDescs, descPrefixes, descSuffixes) : null;
			Align = align;
			if (label != null)
			{
				InsetString = DefaultInsetString;
				Inset = DefaultInset;
			}
			else
			{
				InsetString = string.Empty;
				Inset = 0f;
			}
		}
		
		public DetailSection(string label, List<DefStringTriplet> defStringTriplets, List<StringDescTriplet> stringDescTriplets, bool align = true)
		{
			Label = label;
			KeyDefs = defStringTriplets;
			StringDescs = stringDescTriplets;
			Align = align;
			if (label != null)
			{
				InsetString = DefaultInsetString;
				Inset = DefaultInset;
			}
			else
			{
				InsetString = string.Empty;
				Inset = 0f;
			}
		}

		public DetailSection(string label, List<Def> keyDefs, string[] prefixes = null, string[] suffixes = null, bool align = true)
		{
			Label = label;
			KeyDefs = keyDefs != null ? DetailSectionHelper.BuildDefStringTripletList(keyDefs, prefixes, suffixes) : null;
			StringDescs = null;
			Align = align;
			if (label != null)
			{
				InsetString = DefaultInsetString;
				Inset = DefaultInset;
			}
			else
			{
				InsetString = string.Empty;
				Inset = 0f;
			}
		}
		*/

		public void Initialize()
		{
			FieldDescs = rawFields != null ? DetailSectionHelper.BuildFieldDescList(parentObject, rawFields, parentDesc) : null;
		}

		public void Draw(ref Vector3 cur, float width, DefView window = null)
		{
			if (!initialized)
			{
				Initialize();
				initialized = true;
			}
			if (ParentFieldDescs != null) 
				DoDraw(ref cur, width, window, Label2, ParentFieldDescs);
			DoDraw(ref cur, width, window, Label, FieldDescs);
		}


		public void DoDraw(ref Vector3 cur, float width, DefView window = null, string drawLabel = null, List<FieldDesc> drawFieldDescs = null)
		{
			// Draw section header, if any
			if (!drawLabel.NullOrEmpty())
			{
				Rect labelRect = new Rect(cur.x, cur.y, width, 20f);
				Widgets.Label(labelRect, drawLabel);
				cur.y += 35f - DefExplorerWindow.LineHeightOffset;
			}

			// respect tabs!
			cur.x += Inset;

			// make sure column widths have been calculated
			if (!WidthsSet)
			{
				SetColumnWidths(width - Inset);
			}


			// draw lines one by one
			if (!drawFieldDescs.NullOrEmpty())
			{
				foreach (FieldDesc desc in drawFieldDescs)
				{
					desc.Draw(ref cur, ColumnWidths);
				}
			}
			if (!StringDescs.NullOrEmpty())
			{
				foreach (StringDescTriplet triplet in StringDescs)
				{
					triplet.Draw(ref cur, ColumnWidths);
				}
			}
			if (!KeyDefs.NullOrEmpty())
			{
				foreach (DefStringTriplet triplet in KeyDefs)
				{
					triplet.Draw(ref cur, ColumnWidths, window);
				}
			}

			// add some extra space, reset inset
			cur.y += DefExplorerWindow.ParagraphMargin;
			cur.x -= Inset;
			// done!
		}


		/// <summary>
		/// Take all defined help sections and calculate 'optimal' column widths.
		/// </summary>
		/// <param name="width">Total width</param>
		private void SetColumnWidths(float width)
		{
			// leave some margin
			width -= 2 * _columnMargin;

			// build lists of all strings in this section
			List<string> prefixes = new List<string>();
			List<string> suffixes = new List<string>();
			List<string> descs = new List<string>();
			List<Def> defs = new List<Def>();

			if (StringDescs != null)
			{
				prefixes.AddRange(StringDescs.Select(s => s.Prefix));
				suffixes.AddRange(StringDescs.Select(s => s.Suffix));
				descs.AddRange(StringDescs.Select(s => s.StringDesc));
			}

			if (KeyDefs != null)
			{
				prefixes.AddRange(KeyDefs.Select(k => k.Prefix));
				suffixes.AddRange(KeyDefs.Select(k => k.Suffix));
				defs.AddRange(KeyDefs.Select(k => k.Def));
			}

			if (FieldDescs != null)
			{
				prefixes.AddRange(FieldDescs.Select(f => f.GetTypeString()));
				suffixes.AddRange(FieldDescs.Select(f => FieldDescHelper.GetValueString(f.currentVal)));
				descs.AddRange(FieldDescs.Select(f => f.field.Name));
			}

			if (ParentFieldDescs != null)
			{
				prefixes.AddRange(ParentFieldDescs.Select(f => f.GetTypeString()));
				suffixes.AddRange(ParentFieldDescs.Select(f => FieldDescHelper.GetValueString(f.currentVal)));
				descs.AddRange(ParentFieldDescs.Select(f => f.field.Name));
			}

			// fetch length of all strings, select largest for each column
			Vector3 requestedWidths = Vector3.zero;

			// make sure wrapping is off so we get a true idea of the length
			bool WW = Text.WordWrap;
			Text.WordWrap = false;
			if (!prefixes.NullOrEmpty())
			{
				requestedWidths.x = prefixes.Select(s => Text.CalcSize(s).x).Max() + 10f;
			}
			if (!descs.NullOrEmpty())
			{
				requestedWidths.y = descs.Select(s => Text.CalcSize(s).x).Max() + 2f;
			}
			if (!defs.NullOrEmpty())
			{
				requestedWidths.y = Mathf.Max(requestedWidths.y, defs.Select(d => d.StyledLabelAndIconSize()).Max());
			}
			if (!suffixes.NullOrEmpty())
			{
				requestedWidths.z = suffixes.Select(s => Text.CalcSize(s).x).Max();
			}
			Text.WordWrap = WW;


			if (FieldDescs != null || ParentFieldDescs != null)
			{
				ColumnWidths = requestedWidths;
				ColumnWidths.z += 100f;
			}
			else if (requestedWidths.Sum() < width)
			{
				// expand right-most column (even if it was zero)
				requestedWidths.z += width - requestedWidths.Sum() - 20f;

				// done
				ColumnWidths = requestedWidths;
			}
			else
			{
				// if size overflow is < 30% of largest column width, scale that down.
				if (requestedWidths.Sum() - width < .3f * requestedWidths.Max())
				{
					for (int i = 0; i < 3; i++)
					{
						if (requestedWidths[i] == requestedWidths.Max())
						{
							requestedWidths[i] -= requestedWidths.Sum() - width;
							break;
						}
					}
				}
				else // scale everything down, with a minimum width of 15% per column
				{
					Vector3 shrinkableWidth = requestedWidths.Subtract(width * .15f);
					float scalingFactor = width / shrinkableWidth.Sum();
					for (int i = 0; i < 3; i++)
					{
						requestedWidths[i] -= shrinkableWidth[i] * scalingFactor;
					}
				}

				// done
				ColumnWidths = requestedWidths;
			}
			// set done flag.
			WidthsSet = true;
		}

		/// <summary>
		/// Build the display string for a HelpDetailSection
		/// </summary>
		public string GetString()
		{
			var s = new StringBuilder();
			if (this.Label != null)
			{
				s.AppendLine(this.Label.CapitalizeFirst() + ":");
			}

			if (this.StringDescs != null)
			{
				foreach (StringDescTriplet stringDesc in this.StringDescs)
				{
					s.Append(this.InsetString);
					s.AppendLine(stringDesc.ToString());
				}
			}

			if (this.KeyDefs != null)
			{
				foreach (DefStringTriplet def in this.KeyDefs)
				{
					s.Append(this.InsetString);
					s.AppendLine(def.ToString());
				}
			}

			if (this.FieldDescs != null)
			{
				foreach (FieldDesc def in this.FieldDescs)
				{
					s.Append(this.InsetString);
					s.AppendLine(def.ToString());
				}
			}

			return s.ToString();
		}
	}
}

