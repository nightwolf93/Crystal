using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Kolizeum
{
    public class KolizeumMap
    {
        public int MapID { get; set; }
        public int CellID { get; set; }

        public Database.Records.MapRecords Map
        {
            get
            {
                return Helper.MapHelper.FindMap(this.MapID);
            }
        }

    }
}
