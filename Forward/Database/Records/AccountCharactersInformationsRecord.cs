using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Database.Records
{
    [ActiveRecord("account_characters_informations")]
    public partial class AccountCharactersInformationsRecord : ActiveRecordBase<AccountCharactersInformationsRecord>
    {
        [PrimaryKey("Id")]
        public int ID
        {
            get;
            set;
        }

        [Property("Server")]
        public int Server
        {
            get;
            set;
        }

        [Property("Name")]
        public string Name
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
    }
}
