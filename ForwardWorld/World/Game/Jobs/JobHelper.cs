using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs
{
    public static class JobHelper
    {
        public static Database.Records.CraftRecord GetCraft(int skill, List<Exchange.ExchangeItem> recipe)
        {
            if (Database.Cache.JobDataCache.Cache.FindAll(x => x.Crafts.ContainsKey(skill)).Count > 0)
            {
                var s = Database.Cache.JobDataCache.Cache.FirstOrDefault(x => x.Crafts.ContainsKey(skill));
                foreach (var r in s.Crafts[skill])
                {
                    var c = GetCraftData(r);
                    if (c != null)
                    {
                        if (c.Compare(recipe))
                        {
                            return c;
                        }
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.CraftRecord GetCraftData(int craft)
        {
            if (Database.Cache.CraftCache.Cache.FindAll(x => x.ID == craft).Count > 0)
            {
                return Database.Cache.CraftCache.Cache.FirstOrDefault(x => x.ID == craft);
            }
            else
            {
                return null;
            }
        }

        public static int GetItemBySkill(int skill)
        {
            switch (skill)
            {
                case 6://Frene
                    return 303;

                case 39://Chataigner
                    return 473;

                case 40://Noyer
                    return 476;

                case 10://Chene
                    return 460;

                case 141://Oliviolet
                    return 2357;

                case 139://Bombu
                    return 2358;

                case 37://Erable
                    return 471;

                case 154://Bambou
                    return 7013;

                case 33://If
                    return 461;

                case 41://Merisier
                    return 474;

                case 34://Ebene
                    return 449;

                case 174://Kalyptus
                    return 7925;

                case 155://Bambou sombre
                    return 7016;

                case 38://Charme
                    return 472;

                case 35://Orme
                    return 470;

                case 158://Bambou sacree
                    return 7963;

                case 45://Ble
                    return 289;

                case 53://Orge
                    return 400;

                case 68://Lin
                    return 423;

                default:
                    return -1;
            }
        }
    }
}
