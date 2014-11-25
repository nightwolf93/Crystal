using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class HotelCache
    {
        public static List<Records.HotelRecord> Cache = new List<Records.HotelRecord>();

        public static void Init()
        {
            Cache = Records.HotelRecord.FindAll().ToList();
        }
    }
}
