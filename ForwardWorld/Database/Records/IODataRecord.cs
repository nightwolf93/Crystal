using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("interactive_objects_data")]
    public class IODataRecord : ActiveRecordBase<IODataRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID { get; set; }

        [Property("respawn")]
        public int Respawn { get; set; }

        [Property("duration")]
        public int Duration { get; set; }

        [Property("unknow")]
        public int Unknow { get; set; }

        [Property("walkable")]
        public int Walkable { get; set; }

        [Property("Name")]
        public string Name { get; set; }
    }
}
