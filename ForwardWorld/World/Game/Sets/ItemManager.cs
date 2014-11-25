using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Sets
{
    public static class ItemManager
    {
        public static Database.Records.ItemSetRecord GetSet(int id)
        {
            if (Database.Cache.ItemSetCache.Cache.FindAll(x => x.ID == id).Count > 0)
            {
                return Database.Cache.ItemSetCache.Cache.FirstOrDefault(x => x.ID == id);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.ItemSetRecord GetSetByItem(int item)
        {
            if (Database.Cache.ItemSetCache.Cache.FindAll(x => x.ItemsList.Contains(item)).Count > 0)
            {
                return Database.Cache.ItemSetCache.Cache.FirstOrDefault(x => x.ItemsList.Contains(item));
            }
            else
            {
                return null;
            }
        }
    }
}
