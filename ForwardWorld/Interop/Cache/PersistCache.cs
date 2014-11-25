using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.WorldServer.Interop.Cache
{
    public class PersistCache
    {
        public string Path { get; set; }

        public Dictionary<string, string> Cache = new Dictionary<string, string>();
        public const string CachePath = "Cache/";

        /// <summary>
        /// Usef for load a existing cache file
        /// </summary>
        public PersistCache(string path)
        {
            this.Path = path;
            if (File.Exists(this.Path))
            {
                this.Load();
            }
            else
            {
                this.Save();
            }
        }

        public void Set(string key, string value)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(key))
                {
                    Cache[key] = value;
                }
                else
                {
                    Cache.Add(key, value);
                }
            }
        }

        public void Delete(string key)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(key))
                {
                    Cache.Remove(key);
                }
            }
        }

        public string Get(string key)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(key))
                {
                    return Cache[key];
                }
                else
                {
                    return "null";
                }
            }
        }

        public bool Have(string key)
        {
            lock (Cache)
            {
                return Cache.ContainsKey(key);
            }
        }

        public void Load()
        {
            lock (Cache)
            {
                try
                {
                    var reader = new BinaryReader(File.OpenRead(this.Path));
                    var count = reader.ReadInt32();
                    for (int i = 0; i <= count - 1; i++)
                    {
                        var key = reader.ReadString();
                        var value = reader.ReadString();
                        Cache.Add(key, value);
                    }
                    reader.BaseStream.Close();
                    reader.Close();
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't load persist cache '" + this.Path + "' : " + e.ToString());
                }
            }
        }

        public void Save()
        {
            lock (Cache)
            {
                try
                {
                    var writer = new BinaryWriter(File.Create(this.Path));
                    writer.Write(Cache.Count);
                    foreach (var e in Cache)
                    {
                        writer.Write(e.Key);
                        writer.Write(e.Value);
                    }
                    writer.BaseStream.Close();
                    writer.Close();
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't save persist cache '" + this.Path + "' : " + e.ToString());
                }
            }
        }
    }
}
