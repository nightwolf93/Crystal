using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class BreedHelper
    {
        public static Database.Records.BreedRecord GetBreed(int id)
        {
            return Database.Cache.BreedCache.Cache.FirstOrDefault(x => x.ID == id);
        }
    }
}
