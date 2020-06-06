using DRimEditor.Research;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Noise;
using static DRimEditor.Extensions;
using static DRimEditor.DetailView.FieldDescHelper;
using UnityEngine.UIElements;
using DRimEditor.Windows;

namespace DRimEditor.DetailView
{
    public class FieldDesc
    {
        public object currentVal;

        public string newValText;
        public FieldInfo field;
        object parent;

        private float _height;
        private float _width;
        private bool _heightSet;
        private bool _widthSet;

        public Type childType;
        public string childText;

        private bool expanded = false;
        private bool isAddable;
        private bool childrenHaveDirectChildren = true;
        private bool hasTypeChildren = false;
        private bool foundTypeChildren = false;

        public bool initialized = false;

        public Type[] typeChildren;

        public string command;

        public FieldDesc parentDesc;
        public List<DetailWrapper> childDetailWrappers;
        public List<object> directEditableChildren;

        public FieldDesc(object parent, FieldInfo field, FieldDesc parentDesc = null)
        {
            this.field = field;
            this.parent = parent;
            this.parentDesc = parentDesc;
            newValText = null;
            _height = 0f;
            _heightSet = false;
        }

        public void Initialize()
        {
            Reset();
        }

        public Type GetChildType()  // not dealing with dictionaries etc right now
        {
            if (!IsEnumerable(field.FieldType))
                return null;
            Type[] types = field.FieldType.GenericTypeArguments;
            childText = "null";

            if (types == null || types.Count() == 0)
                return null;
            FieldInfo[] directFields = GetAllDirectEditableFields(types[0]);
            if (directFields.Count() <= 0)
                childrenHaveDirectChildren = false;
            return types[0];
        }


        public void Reset()
        {
            currentVal = GetValue();
            newValText = GetValueString(currentVal);
            string[] widthLines = newValText.Split('\n');
            _height = CalcHeight(widthLines, Text.LineHeight);
            _width = CalcWidth(widthLines, IsDirectEditable(field.FieldType));
            childType = GetChildType();
            
            parentDesc?.Reset();
        }

        public void GetChildren()
        {
            if (!IsEnumerable(field.FieldType))
            {
                Verse.Log.Error("Tried to get children when not collection");
                return;
            }
            var valCollection = currentVal as IEnumerable;
            var enumerator = valCollection.GetEnumerator();

            if (IsDirectEditable(childType))
            {
                directEditableChildren = new List<object>();
                while (enumerator.MoveNext())
                {
                    object cur = enumerator.Current;
                    directEditableChildren.Add(cur);
                }
            }
            else if (!IsEnumerable(childType))
            {
                childDetailWrappers = new List<DetailWrapper>();
                while (enumerator.MoveNext())
                {
                    object cur = enumerator.Current;
                    DetailWrapper details = new DetailWrapper(cur, null, true, this);
                    childDetailWrappers.Add(details);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(field.Name);
            return s.ToString();
        }

        public string GetTypeString()
        {
            return field.FieldType.ToString();
        }

        public object GetValue()
        {
            return field.GetValue(parent);
        }

        public string MakeSetCommand(object obj)
        {
            if (obj == null) // either it's null..
            {
                return "".Set(field).FindNull();
            }

            Type setType = obj.GetType();
            if (IsDirectEditable(setType)) //  we can find it...
            {
                return "".Set(field).Find(obj);
            }
            else // or we made a new one
            {
                return "".Set(field).New(setType);
            }

        }

        public void AddCommand(string childstr = null)
        {
            string toAdd = command;

            if (childstr != null)
            {
                    toAdd += "".Get(field);
                    toAdd += ":" + childstr;
            }

            if (parentDesc != null)
            {
                if (IsEnumerable(parentDesc.field.FieldType)) // we are a member of an enumerable
                {
                    string prepend = "";
                    // currentVal is us, the member of the enumerable
                    if (IsDirectEditable(parent.GetType())) // but wait, why are we here?
                    {
                        Verse.Log.Error("RimEditor: Trying to cascade down a directly editable stack; should be directly editing from dropdown menu");
                        //toAdd = prepend.From().Find(currentVal) + ":" + toAdd;
                    }
                    else
                    {
                        GenerateFromCommand(parent, ref prepend);
                        toAdd = prepend + ":" + toAdd;
                    }
                }
                parentDesc.AddCommand(toAdd);
            }
            else if (typeof(Def).IsAssignableFrom(parent.GetType()))
            {
                toAdd = Find(parent) + ":" + toAdd;
                ProfileManager.AddCommand(toAdd);
            }
            else
            {
                Verse.Log.Error("RimEditor: Tried to add command with unknown parent of type " + parent.GetType());
            }

            command = null;
        }

        public bool TryMakeNew(Type type, bool final = true)
        {
            if (!CanMakeNew(type))
            {
                Verse.Log.Error("Tried to make new object of type that that can't be insantiated, type was " + type.Name);
                Reset();
                return false;
            }
            
            object newObj = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            return TrySetValue(newObj, final); // will set command
        }

        public bool TryDelete(bool final = true)
        {
            if (IsDirectEditable(field.FieldType) || currentVal == null)
            {
                Verse.Log.Error("Tried to remove object that is either null or directly editable");
                Reset();
                return false;
            }
            this.childDetailWrappers = null;
            this.directEditableChildren = null;
            this.expanded = false;
            return TrySetValue(null, final);
        }

        public bool TryRemoveIndirect(DetailWrapper childSection, bool final = true)
        {
            if (!IsEnumerable(field.FieldType))
            {
                Verse.Log.Error("Tried to remove indirect from non-collection");
                Reset();
                return false;
            }
            if(!childrenHaveDirectChildren)
            {
                Verse.Log.Error("Tried to remove indirect but no direct editable fields");
                Reset();
                return false;
            }
            object targetObj = childSection.parentObject; // of childType

            // make command, have to compare directly editable field values and hope we get the right object
            command = "".Get(field).Remove();
            GenerateFromCommand(targetObj, ref command);
            if (final)
                AddCommand();
            Type collectionType = field.FieldType;
            MethodInfo remove = AccessTools.Method(collectionType, "Remove");
            remove.Invoke(currentVal, new object[] { targetObj });
            childDetailWrappers.Remove(childSection);
            return true;
        }

        public bool TryRemoveDirect(object obj, bool final = true) 
        {
            if (!IsEnumerable(field.FieldType))
            {
                Verse.Log.Error("Tried to remove direct from non-collection");
                Reset();
                return false;
            }
            command = "".Get(field).Remove().Find(obj);
            if (final)
                AddCommand();
            Type collectionType = field.FieldType;
            MethodInfo remove = AccessTools.Method(collectionType, "Remove");
            remove.Invoke(currentVal, new object[] { obj });
            directEditableChildren.Remove(obj);
            //Reset();
            return true;
        }

        public bool TryAddNewDirect(object obj, bool final = true)
        {
            if (!IsEnumerable(field.FieldType))
            {
                Verse.Log.Error("Tried to add direct to non-collection");
                Reset();
                return false;
            }
            command = "".Get(field).Add().Find(obj);
            if (final)
                AddCommand();
            Type collectionType = field.FieldType;
            MethodInfo add = AccessTools.Method(collectionType, "Add");
            add.Invoke(currentVal, new object[] { obj });
            directEditableChildren.Add(obj);
            //Reset();
            return true;
        }

        public bool TryAddNewIndirect(bool final = true)
        {
            if (!IsEnumerable(field.FieldType))
            {
                Verse.Log.Error("Tried to add indirect to non-collection");
                Reset();
                return false;
            }
            if (!isAddable)
            {
                Verse.Log.Error("Tried to make new object of type that that can't be insantiated, in addNewIndirect");
                Reset();
                return false;
            }

            object newObj = Activator.CreateInstance(childType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            Type collectionType = field.FieldType;
            MethodInfo add = AccessTools.Method(collectionType, "Add");
            add.Invoke(currentVal, new object[] { newObj });
            DetailWrapper details = new DetailWrapper(newObj, null, true, this);
            childDetailWrappers.Add(details);

            command = "".Get(field).Add().New(childType);
            if (final)
                AddCommand();
            return true;
        }

        public bool TrySetValue(object obj, bool final = true)
        {
            if (obj is string stringObj)
            {
                if (!IsConvertibleFromText(field.FieldType))
                {
                    Verse.Log.Error("Tried to convert from text to object not convertible");
                    Reset();
                    return false;
                }
                object newVal;
                if (stringObj == "null")
                {
                    newVal = null;
                }
                else
                {
                    try
                    {
                        newVal = Convert.ChangeType(stringObj, field.FieldType);
                    }
                    catch
                    {
                        Dialog_MessageBox invalid = new Dialog_MessageBox("Invalid value for " + field.FieldType);
                        Verse.Find.WindowStack.Add(invalid);
                        Reset();
                        return false;
                    }
                }
                command = MakeSetCommand(newVal);
                if (final)
                    AddCommand();
                field.SetValue(parent, newVal);
                Reset();
                return true;
            }
            else 
            {
                if (obj != null && !field.FieldType.IsAssignableFrom(obj.GetType()))
                {
                    Verse.Log.Error("Tried to assign object of wrong type");
                    Reset();
                    return false;
                }
                command = MakeSetCommand(obj);
                if (final)
                    AddCommand();
                field.SetValue(parent, obj);
                Reset();
                if (parent is ResearchProjectDef)
                {
                    ResearchWindow.Refresh();
                }
                return true;
            }
        }   

        public void DrawLabels(ref Vector3 cur, Vector3 colWidths, float lineHeight, string first, string second)
        {
            if (first != null)
            {
                Rect prefixRect = new Rect(cur.x, cur.y, colWidths.x, lineHeight);
                Widgets.Label(prefixRect, first);
                cur.x += colWidths.x;
            }

            if (second != null)
            {
                Rect labelRect = new Rect(cur.x, cur.y, colWidths.y, lineHeight);
                Widgets.Label(labelRect, second);
                cur.x += colWidths.y + 2 * DetailWrapper._columnMargin;
            }
        }

        public void DrawDirectEditable(ref Vector3 cur, float lineHeight, ref float width, ref float height, ref string valText, string originalText, Type objectType)
        {
            // Editable ------------------------------------------------------------- 
            string[] lines = newValText.Split('\n');
            bool changed = false;
            Rect modifyRect;

            if (IsConvertibleFromText(objectType))
            {
                modifyRect = new Rect(cur.x, cur.y, width, lineHeight * lines.Count());
                if (Mouse.IsOver(modifyRect) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    DefExplorerWindow.TryChangeActiveDesc(this);
                }
                string newerValText;
                newerValText = Widgets.TextArea(modifyRect, valText, readOnly: false);
                if (newerValText != valText)
                {
                    valText = newerValText;
                    height = CalcHeight(lines, lineHeight);
                    width = CalcWidth(lines, true);
                }
                changed = valText != originalText;
                if (changed)
                {
                    Rect setButtonrect = new Rect(cur.x, cur.y + height, 30f, lineHeight);
                    bool set = Widgets.ButtonText(setButtonrect, "Set");
                    if (set)
                    {
                        TrySetValue(valText);
                    }
                }
                
            }
            else
            {
                modifyRect = new Rect(cur.x, cur.y, width, lineHeight * lines.Count());
                if (Mouse.IsOver(modifyRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1 && typeof(Def).IsAssignableFrom(field.FieldType) && currentVal != null)
                {
                    var options = new List<FloatMenuOption>();
                    options.Add(new FloatMenuOption("Jump to " + GetValueString(currentVal), () => RightClickHandler(currentVal), MenuOptionPriority.High, null));
                    Verse.Find.WindowStack.Add(new FloatMenu(options));
                    Event.current.Use();
                }
                Widgets.Dropdown<Type, string>(modifyRect, objectType, GetDropdownPayload, GetDropdownMenu, buttonLabel: originalText);
            }
            cur.x += width;
            cur.y += (changed ? lineHeight + 8f : 0f);
            cur.y += _height - DefExplorerWindow.LineHeightOffset;
        }


        public void DrawIndirectEditable(ref Vector3 cur, float lineHeight, float width, float height, string valText)
        {
            if (!foundTypeChildren)
            {
                typeChildren = DatabaseBuilder.AllTypeSubclasses(field.FieldType);
                hasTypeChildren = typeChildren != null && typeChildren.Count() > 0;
                foundTypeChildren = true;

                if (childType != null)
                    isAddable = CanMakeNew(childType);
                else
                {
                    if (hasTypeChildren)
                    {
                        isAddable = typeChildren.Any(x => CanMakeNew(x));
                    }
                    else
                        isAddable = CanMakeNew(field.FieldType);
                }
            }
            
            Rect changeRect = new Rect(cur.x, cur.y, lineHeight, lineHeight);
            if (currentVal == null) // can add new
            {
                
                if (isAddable)
                {
                    if (hasTypeChildren)
                    {
                        changeRect.width = width;
                        Widgets.Dropdown<Type, string>(changeRect, field.FieldType, GetTypePayload, GetTypeDropdown, buttonLabel: "Add");
                    }
                    else
                    {
                        bool add = Widgets.ButtonText(changeRect, "+");
                        if (add)
                        {
                            TryMakeNew(field.FieldType, true);
                        }
                    }
                }
            }
            else // can delete object, or expand
            {
                bool delete = Widgets.ButtonText(changeRect, "-");
                if (delete)
                {
                    TryDelete(true);
                }
            }
            cur.x += lineHeight + 2f;

            Rect valueRect = new Rect(cur.x, cur.y, width, height);
            if (currentVal != null || !hasTypeChildren)
            {
                Widgets.TextArea(valueRect, valText, readOnly: true);
            }
            float arrowSize = lineHeight / 2f;
            

            if (currentVal != null && childrenHaveDirectChildren)
            {
                Rect expandRect = new Rect(cur.x + valueRect.width, cur.y + arrowSize / 2f, arrowSize, arrowSize);
                bool expand = Widgets.ButtonImage(expandRect, expanded ? ResourceBank.Icon.HelpMenuArrowDown : ResourceBank.Icon.HelpMenuArrowRight);
                if (expand)
                {
                    expanded = !expanded;
                }
            }
            cur.y += _height - DefExplorerWindow.LineHeightOffset;
            if (expanded)
            {
                float xPreChildren = cur.x + valueRect.width + arrowSize;
                DrawExpanded(ref cur, lineHeight);
                cur.x = Math.Max(xPreChildren, cur.x);
            }
            else
            {
                cur.x += valueRect.width;
            }
        }


        public void DrawDirectAdd(ref Vector3 cur, float lineHeight)
        {
            string[] lines = childText.Split('\n');
            Rect modifyRect;
            float width = CalcWidth(lines, true);
            if (IsConvertibleFromText(childType))
            {
                Rect directAddRect = new Rect(cur.x, cur.y, lineHeight, lineHeight);
                bool add = Widgets.ButtonText(directAddRect, "+");
                if (add)
                {
                    TryAddNewDirect(childText);
                }
                cur.x += lineHeight + 4f;
                modifyRect = new Rect(cur.x, cur.y, width, lineHeight * lines.Count());
                if (Mouse.IsOver(modifyRect) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    DefExplorerWindow.TryChangeActiveDesc(this);
                }
                string newerChildText;
                newerChildText = Widgets.TextArea(modifyRect, childText, readOnly: false);
                if (newerChildText != childText)
                {
                    childText = newerChildText;
                }
                cur.x += 20f; // buffer for bigass textbox or button
            }
            else
            {
                modifyRect = new Rect(cur.x, cur.y, width, lineHeight * lines.Count());
                Widgets.Dropdown<Type, string>(modifyRect, childType, GetAddDropdownPayload, GetDropdownMenu, buttonLabel: "Add");
            }
            cur.x += width;
        }


        public void DrawExpanded(ref Vector3 cur, float lineHeight)
        {
            if (!IsEnumerable(field.FieldType))
            {
                if (childDetailWrappers == null)
                {
                    childDetailWrappers = new List<DetailWrapper>();
                    DetailWrapper details = new DetailWrapper(currentVal, null, true, this);
                    childDetailWrappers.Add(details);
                }
                childDetailWrappers[0].Draw(ref cur, cur.z);
            }
            else
            {

                float oldx = cur.x;
                float newz = cur.x;
                bool directEditable = IsDirectEditable(childType);
                if ((!directEditable && childDetailWrappers == null) || (directEditable && directEditableChildren == null))
                {
                    GetChildren();
                }
                if ((!directEditable && childDetailWrappers.Count == 0) || (directEditable && directEditableChildren.Count == 0))
                {
                    string emptyString = "(empty)";
                    float emptyWidth = Text.CalcSize(emptyString).x;
                    DrawLabels(ref cur, new Vector3(0, emptyWidth, 0), lineHeight, null, emptyString);
                    cur.y += lineHeight;
                    newz = Mathf.Max(newz, cur.x);
                    cur.x = oldx;
                }
                if (directEditable)
                {

                    foreach (object child in directEditableChildren ?? Enumerable.Empty<object>())
                    {
                        Rect changeRect = new Rect(cur.x, cur.y, lineHeight, lineHeight);
                        bool delete = Widgets.ButtonText(changeRect, "-");
                        if (delete)
                        {
                            if (TryRemoveDirect(child, true))
                            {
                                break;
                            }
                        }
                        cur.x += lineHeight + 2f;

                        Rect valRect = new Rect(cur.x, cur.y, 1f, lineHeight);
                        string typeString = child.GetType().ToString();
                        float typeWidth = Text.CalcSize(typeString).x + 10f;
                        string valString = GetValueString(child);
                        float valWidth = Text.CalcSize(valString).x;

                        Vector3 childWidths = new Vector3(typeWidth, valWidth, 0);
                        DrawLabels(ref cur, childWidths, lineHeight, typeString, valString);

                        valRect.width = (cur.x - valRect.x);     
                        if (Mouse.IsOver(valRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1 && typeof(Def).IsAssignableFrom(childType) && currentVal != null)
                        {
                            var options = new List<FloatMenuOption>();
                            options.Add(new FloatMenuOption("Jump to " + GetValueString(child), () => RightClickHandler(child), MenuOptionPriority.High, null));
                            Verse.Find.WindowStack.Add(new FloatMenu(options));
                            Event.current.Use();
                        }

                        cur.y += lineHeight;
                        newz = Mathf.Max(newz, cur.x);
                        cur.x = oldx;
                    }
                    DrawDirectAdd(ref cur, lineHeight);
                    cur.y += lineHeight;
                    newz = Mathf.Max(newz, cur.x);
                    cur.x = oldx;
                }
                else
                {
                    foreach (DetailWrapper child in childDetailWrappers ?? Enumerable.Empty<DetailWrapper>())
                    {
                        Rect changeRect = new Rect(cur.x, cur.y, lineHeight, lineHeight);
                        bool delete = Widgets.ButtonText(changeRect, "-");
                        if (delete)
                        {
                            if (TryRemoveIndirect(child, true))
                            {
                                break;
                            }
                        }
                        cur.x += lineHeight + 2f;

                        string typeString = child.parentObject.GetType().ToString();
                        float typeWidth = Text.CalcSize(typeString).x + 10f;
                        string valString = GetValueString(child.parentObject);
                        float valWidth = Text.CalcSize(valString).x;
                        Vector3 childWidths = new Vector3(typeWidth, valWidth, 0);
                        DrawLabels(ref cur, childWidths, lineHeight, typeString, valString);

                        float arrowSize = lineHeight / 2f;
                        Rect expandRect = new Rect(cur.x, cur.y + arrowSize / 2f, arrowSize, arrowSize);
                        bool expand = Widgets.ButtonImage(expandRect, child.expanded ? ResourceBank.Icon.HelpMenuArrowDown : ResourceBank.Icon.HelpMenuArrowRight);
                        if (expand)
                        {
                            child.expanded = !child.expanded;
                        }

                        cur.y += lineHeight;
                        newz = Mathf.Max(newz, cur.x);
                        cur.x = oldx;
                        if (child.expanded)
                        {
                            child.Draw(ref cur, cur.z);
                        }
                    }
                    if (isAddable)
                    {
                        Rect detailAddRect = new Rect(cur.x, cur.y, lineHeight, lineHeight);
                        bool add = Widgets.ButtonText(detailAddRect, "+");
                        if (add)
                        {
                            TryAddNewIndirect(true);
                        }
                        cur.y += lineHeight;
                    }
                    cur.x = newz;
                }
            }
        }

        public void RightClickHandler(object obj)
        {

            FieldDesc m = this;
            while (m.parentDesc != null)
                m = m.parentDesc;
            Def oldItem = m.parent as Def;
            MainWindow.backStack.Push(() => DefExplorerWindow.instance.JumpTo(oldItem));
            DefExplorerWindow.instance.JumpTo(obj as Def);
        }

        public void Draw(ref Vector3 cur, Vector3 colWidths)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            float lineHeight = Text.LineHeight;
            if (!_heightSet)
            {
                string[] lines = newValText.Split('\n');
                _height = CalcHeight(lines, lineHeight);
                _heightSet = true;
            }
            if (!_widthSet)
            {
                string[] widthLines = newValText.Split('\n');
                _width = CalcWidth(widthLines, IsDirectEditable(field.FieldType));
                _widthSet = true;
            }
            if (newValText == null)
            {
                Reset();
            }

            float oldx = cur.x;

            DrawLabels(ref cur, colWidths, lineHeight, GetTypeString(), field.Name);

            if (IsDirectEditable(field.FieldType))
                DrawDirectEditable(ref cur, lineHeight, ref _width, ref _height, ref newValText, GetValueString(currentVal), field.FieldType);
            else
                DrawIndirectEditable(ref cur, lineHeight, _width, _height, newValText);

            // Wrapup ------------------------------------------------------------- 

            cur.z = Mathf.Max(cur.z, cur.x + 50f);
            cur.x = oldx;
        } 

        public IEnumerable<Widgets.DropdownMenuElement<string>> GetDropdownMenu(Type type)
        {
            if (!IsEnumerable(field.FieldType))
            {
                return MakeDropdownMenu(type, (x) => TrySetValue(x));
            }
            else
            {
                if (IsDirectEditable(type))
                    return MakeDropdownMenu(type, (x) => TryAddNewDirect(x));
                //else
                //    return MakeDropdownMenu(type, (x) => TryAddNewIndirect()); // but what are we doing here????
                throw new Exception("Tried to create dropdown menu for ineligible type (not direct assignable or enumerable of direct assignables)");
            }
        }

        public static string GetTypePayload(Type type)
        {
            return "";
        }

        public IEnumerable<Widgets.DropdownMenuElement<string>> GetTypeDropdown(Type type)
        {
            List<Widgets.DropdownMenuElement<string>> options = new List<Widgets.DropdownMenuElement<string>>();
            
            foreach(Type subclass in typeChildren)
            {
                if (CanMakeNew(subclass))
                {
                    options.Add(new Widgets.DropdownMenuElement<string>
                    {
                        option = new FloatMenuOption(subclass.Name, () => TryMakeNew(subclass), MenuOptionPriority.Default),
                        payload = subclass.Name
                    });
                }
            }

            options.SortBy(x => x.payload);
            return options;
        }
      
    }
}