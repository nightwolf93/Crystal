using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.WorldServer.World.Game.Exchange
{
    public static class ExchangeRestrictions
    {
        public static List<int> RestrictedItems = new List<int>();

        public static void LoadRestrictedItems()
        {
            if (File.Exists("Datas/ExchangeRestriction.txt"))
            {
                var reader = new StreamReader("Datas/ExchangeRestriction.txt");
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.ToLower().StartsWith("items"))
                    {
                        string[] data = line.ToLower().Split('=');
                        foreach (string item in data[1].Trim().Split(','))
                        {
                            if (item != "")
                            {
                                RestrictedItems.Add(int.Parse(item.Trim()));
                            }
                        }
                    }
                }
                reader.Close();
            }
        }
    }
}
