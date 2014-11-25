using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class SpellHelper
    {
        public static Database.Records.SpellRecord GetSpell(int id)
        {
            if (Database.Cache.SpellCache.Cache.FindAll(x => x.ID == id).Count > 0)
            {
                return Database.Cache.SpellCache.Cache.FirstOrDefault(x => x.ID == id);
            }
            else
            {
                return null;
            }
        }
    }
}
