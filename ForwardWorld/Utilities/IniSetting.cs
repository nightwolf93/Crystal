using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.WorldServer.Utilities
{
    public class IniSetting
    {
        public string Path = ("");
        public Dictionary<string, Dictionary<string, string>> Elements = new Dictionary<string, Dictionary<string, string>>();

        public IniSetting(string path)
        {
            this.Path = path;
        }

        public Dictionary<string, string> GetGroup(string group)
        {
            return this.Elements[group];
        }

        public string GetStringElement(string group, string key)
        {
            return GetGroup(group)[key];
        }

        public int GetIntElement(string group, string key)
        {
            return int.Parse(GetGroup(group)[key]);
        }

        public bool GetBoolElement(string group, string key)
        {
            return bool.Parse(GetGroup(group)[key]);
        }

        public bool ContainsGroup(string key)
        {
            return this.Elements.ContainsKey(key);
        }

        public Dictionary<string, string> GetFirstGroup()
        {
            return this.Elements.FirstOrDefault().Value;
        }

        public void Save()
        {
            StreamWriter writer = new StreamWriter(this.Path);
            foreach (var group in this.Elements)
            {
                writer.WriteLine("[" + group.Key + "]");
                foreach (var value in group.Value)
                {
                    writer.WriteLine(value.Key + " = " + value.Value);
                }
            }
            writer.Close();
        }

        public void ReadSettings()
        {
            this.Elements.Clear();
            Dictionary<string, string> currentGroup = null;
            StreamReader reader = new StreamReader(this.Path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line != "" && !line.StartsWith("#"))
                {
                    if (line.StartsWith("["))
                    {
                        currentGroup = new Dictionary<string, string>();
                        this.Elements.Add(line.Replace("[", "").Replace("]", ""), currentGroup);
                    }
                    else if (currentGroup != null)
                    {
                        string[] data = line.Trim().Split('=');
                        string key = data[0].Trim();
                        string value = data[1].Trim();
                        currentGroup.Add(key, value);
                    }
                }
            }
            reader.Close();
        }
    }
}
