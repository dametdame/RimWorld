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
        List<string> loaded = new List<string>();
        Dictionary<string, string> errored = new Dictionary<string, string>();

        public List<string> getLoaded
        {
            get
            {
                return loaded;
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
        }
        public void AddCommand(string line)
        {
            loaded.Add(line);
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
