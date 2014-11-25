using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class GuildCreatorLocationCache
    {
        public static List<Records.GuildCreatorLocationRecord> Cache = new List<Records.GuildCreatorLocationRecord>();

        public static void Init()
        {
            Cache = Records.GuildCreatorLocationRecord.FindAll().ToList();
        }
    }
}
