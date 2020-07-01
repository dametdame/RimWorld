using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Reflection;
using System.IO;

namespace DRimEditor
{
    public class Profile
    {
        string label;
        public FileInfo file;
        public List<string> loaded = new List<string>();
        Dictionary<string, string> errored = new Dictionary<string, string>();
        private Dictionary<string, float> heights = new Dictionary<string, float>();

        private float cachedHeight = -1f;
        private float cachedWidth = -1f;
        public bool heightChanged = true;

        public float GetHeight(float width, bool onlyErrored = false)
        {
            setHeights(width, onlyErrored);
            return cachedHeight;
        }

        public float CommandHeight(string command, float width)
        {
            setHeights(width);
            if (heights.ContainsKey(command))
                return heights[command];
            else
                return 0f;
        }

        public void setHeights(float width, bool onlyErrored = false)
        {
            if (heightChanged || width != cachedWidth || cachedHeight < 0)
            {
                heights = new Dictionary<string, float>();
                float height = 0f;
                foreach (string command in loaded)
                {
                    float commandHeight = Text.CalcHeight(Profile.FormatComand(command), width);
                    heights.SetOrAdd(command, commandHeight);
                    if (!onlyErrored || errored.ContainsKey(command))
                        height += commandHeight;
                }
                cachedHeight = height;
                cachedWidth = width;
                heightChanged = false;
            }
        }

        public Profile(FileInfo sourceFile)
        {
            label = sourceFile.Name;
            this.file = sourceFile;
            Load();
        }

        public override string ToString()
        {
            return label;
        }

        public static string FormatComand(string s)
        {
            string newstring = s;
            newstring = newstring.Replace("_", "");
            newstring = newstring.Replace(":", " ");
            return newstring;
        }

        public string PrintDetails()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in loaded)
            {
                sb.AppendLine(FormatComand(s));
            }
            return sb.ToString();
        }

        public void DeleteCommand(int index)
        {
            loaded.RemoveAt(index);
            ProfileManager.SetChanged();
        }

        public void DeleteCommand(string command)
        {
            loaded.Remove(command);
            ProfileManager.SetChanged();
        }

        public void AddCommand(string line)
        {
            loaded.Add(line);
            ProfileManager.SetChanged();
        }

        public bool HasError(string line)
        {
            return errored.ContainsKey(line);
        }

        public string GetError(string line)
        {
            return errored.TryGetValue(line);
        }

        public void Load()
        {
            try
            {
                if (!file.Exists)
                {
                    return;
                }
                
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        loaded.Add(line);
                    }
                }  
            }
            catch (Exception e)
            {
                Log.Error("Error loading profile " + label + ", " + e.Message);
            }
            heightChanged = true;
        }

        public void Save()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file.FullName))
                {
                    foreach(string s in loaded)
                    {
                        sw.WriteLine(s);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error saving profile " + label + ", " + e.Message);
            }
        }

        public void Apply()
        {
            Log.Message("RimEditor: Applying profile " + label);
            Parser.queue.Clear();
            Parser.stack.Clear();
            foreach (string line in loaded)
            {
                try
                {
                    Parser.notFinished = true;
                    if (line.NullOrEmpty())
                        continue;
                    Parser.StartParse(line);
                }
                catch (Exception e)
                {
                    Log.Error("RimEditor: Could not apply command " + line);
                    Log.Error("RimEditor: Error was " + e.Message);
                    errored.SetOrAdd(line, e.Message);
                }
            }
        }
    }
}
