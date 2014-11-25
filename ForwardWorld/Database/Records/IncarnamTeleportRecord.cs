using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("incarnam_teleporters")]
    public class IncarnamTeleportRecord : ActiveRecordBase<IncarnamTeleportRecord>
    {
        [PrimaryKey("ID")]
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

        [Property("CellID")]
        public int CellID
        {
            get;
            set;
        }

        [Property("MaxLevel")]
        public int MaxLevel
        {
            get;
            set;
        }
    }
}
