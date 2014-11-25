using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : PaddockRecord
*/

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("paddocks_db")]
    public class PaddockRecord : ActiveRecordBase<PaddockRecord>
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

        [Property("CellID")]
        public int CellID
        {
            get;
            set;
        }

        [Property("IsPublic")]
        public bool IsPublic
        {
            get;
            set;
        }

        [Property("Alignment")]
        public int Alignment
        {
            get;
            set;
        }

        [Property("Owner")]
        public int Owner
        {
            get;
            set;
        }

        [Property("Capacity")]
        public int Capacity
        {
            get;
            set;
        }

        [Property("Objects")]
        public string Objects
        {
            get;
            set;
        }

        [Property("Cost")]
        public int Cost
        {
            get;
            set;
        }
    }
}
