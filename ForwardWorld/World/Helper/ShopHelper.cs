using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class ShopHelper
    {
        public static Database.Records.ShopItemRecord FindShopItem(int id)
        {
            if (Database.Cache.ShopItemCache.Cache.FindAll(x => x.ID == id).Count > 0)
            {
                return Database.Cache.ShopItemCache.Cache.FirstOrDefault(x => x.ID == id);
            }
            return null;
        }
    }
}
