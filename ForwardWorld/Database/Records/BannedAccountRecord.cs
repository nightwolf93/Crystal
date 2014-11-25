using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("banned_accounts")]
    public class BannedAccountRecord : ActiveRecordBase<BannedAccountRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID
        {
            get;
            set;
        }

        [Property("account")]
        public string Account
        {
            get;
            set;
        }
    }
}
