using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("maps_dungeon_rooms")]
    public class DungeonRoomRecord : ActiveRecordBase<DungeonRoomRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("MapID")]
        public int MapID
        {
            get;
            set;
        }

        [Property("ToMap")]
        public int ToMap
        {
            get;
            set;
        }

        [Property("ToCell")]
        public int ToCell
        {
            get;
            set;
        }
    }
}
