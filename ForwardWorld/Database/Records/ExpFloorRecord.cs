using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("exp_floors")]
    public class ExpFloorRecord : ActiveRecordBase<ExpFloorRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Characters")]
        public long Character
        {
            get;
            set;
        }

        [Property("Job")]
        public int Job
        {
            get;
            set;
        }

        [Property("Mount")]
        public int Mount
        {
            get;
            set;
        }

        [Property("Pvp")]
        public int Pvp
        {
            get;
            set;
        }
    }
}
