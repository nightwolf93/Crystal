using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.WorldServer.World.Game.Pvp
{
    public class PvpManager
    {
        public static List<int> NoPvpMaps = new List<int>();

        public static void LoadNoPvpMaps()
        {
            NoPvpMaps.Clear();
            if (File.Exists("Datas/Pvp.txt"))
            {
                Utilities.ConsoleStyle.Infos("Loading @no pvp maps@ ...");
                var reader = new StreamReader("Datas/Pvp.txt");
                var str = reader.ReadToEnd();
                reader.Close();
                if (str != "")
                {
                    foreach (var m in str.Split(','))
                    {
                        try
                        {
                            if (m != "")
                            {
                                NoPvpMaps.Add(int.Parse(m));
                            }
                        }
                        catch (Exception e)
                        {
                            Utilities.ConsoleStyle.Error("Can't load no pvp maps : " + e.ToString());
                        }
                    }
                }
                Utilities.ConsoleStyle.Infos("Loaded @'" + NoPvpMaps.Count + "'@ no pvp maps !");
            }
            else
            {
                Utilities.ConsoleStyle.Warning("No pvp maps restrictions founded !");
            }
        }

        public static bool IsNoPvpMap(int id)
        {
            return NoPvpMaps.Contains(id);
        }
    }
}
