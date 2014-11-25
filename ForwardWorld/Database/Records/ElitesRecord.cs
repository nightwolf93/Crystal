using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("elites")]
    public class ElitesRecord : ActiveRecordBase<ElitesRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("name")]
        public string Name
        {
            get;
            set;
        }

        [Property("level")]
        public int Level
        {
            get;
            set;
        }

        [Property("bonus")]
        public string Bonus
        {
            get;
            set;
        }

        [Property("titleid")]
        public int TitleID
        {
            get;
            set;
        }
    }
}
