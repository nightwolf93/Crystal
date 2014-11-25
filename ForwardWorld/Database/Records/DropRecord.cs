using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : DropRecord
*/

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("drops_db")]
    public class DropRecord : ActiveRecordBase<DropRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("mob")]
        public int MonsterID
        {
            get;
            set;
        }

        [Property("item")]
        public int ItemID
        {
            get;
            set;
        }

        [Property("seuil")]
        public int Floor
        {
            get;
            set;
        }

        [Property("max")]
        public int Quantity
        {
            get;
            set;
        }

        [Property("taux")]
        public int Rate
        {
            get;
            set;
        }
    }
}
