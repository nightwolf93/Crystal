using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class GuildHelper
    {
        public static bool ExistGuild(string name)
        {
            if (Database.Cache.GuildCache.Cache.FindAll(x => x.Name.ToLower() == name.ToLower()).Count > 0)
            {
                return true;
            }
            return false;
        }

        public static Game.Guilds.Guild GetGuild(int id)
        {
            if (Database.Cache.GuildCache.Cache.FindAll(x => x.GuildData.ID == id).Count > 0)
            {
                return Database.Cache.GuildCache.Cache.FirstOrDefault(x => x.GuildData.ID == id);
            }
            return null;
        }
    }
}
