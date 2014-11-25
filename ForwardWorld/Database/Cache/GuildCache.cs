using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class GuildCache
    {
        public static List<World.Game.Guilds.Guild> Cache = new List<World.Game.Guilds.Guild>();

        public static void Init()
        {
            Records.GuildRecord.FindAll().ToList().ForEach(x => Cache.Add(
                new World.Game.Guilds.Guild
                (x.ID, x.Name, new World.Game.Guilds.GuildEmblem
                    (x.EmblemBackID, x.EmblemBackColor, x.EmblemFrontID, x.EmblemFrontColor), x)));
        }
    }
}
