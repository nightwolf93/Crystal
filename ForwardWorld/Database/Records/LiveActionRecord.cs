using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("live_action")]
    public class LiveActionRecord : ActiveRecordBase<LiveActionRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("PlayerID")]
        public int PlayerID
        {
            get;
            set;
        }

        [Property("Action")]
        public int Action
        {
            get;
            set;
        }

        [Property("Nombre")]
        public int Nombre
        {
            get;
            set;
        }
    }
}
