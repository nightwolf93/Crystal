using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Crystal.WorldServer.World.Game.Admin
{
    public class AdminRankManager
    {
        public static Dictionary<int, AdminRank> Ranks = new Dictionary<int, AdminRank>();

        public static void Initialize()
        {
            if (File.Exists("Datas/AdminsRanks.txt"))
            {
                var reader = new StreamReader("Datas/AdminsRanks.txt");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != "")
                    {
                        if (!line.StartsWith("#"))
                        {
                            var data = line.Split(';');
                            var id = int.Parse(data[0]);
                            var name = data[1];
                            var superadmin = data[2].ToLower() == "yes";
                            var rights = data[3].Split(',');
                            var rank = new AdminRank()
                            {
                                RankID = id,
                                Name = name,
                                SuperAdmin = superadmin,
                            };
                            foreach (var r in rights)
                            {
                                if (r != "")
                                {
                                    rank.Permissions.Add(r.ToLower());
                                }
                            }
                            Ranks.Add(id, rank);
                            Utilities.ConsoleStyle.Infos("Admins rank '" + name + "' loaded !");
                        }
                    }
                }
                reader.Close();
            }
            else
            {
                Utilities.ConsoleStyle.Error("Can't admins ranks permissions file .. please create Datas/AdminsRanks.txt");
            }
        }

        public static AdminRank GetRank(int id)
        {
            if (Ranks.ContainsKey(id))
            {
                return Ranks[id];
            }
            else
            {
                return null;
            }
        }
    }
}
