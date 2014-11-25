using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("base_spells")]
    public class BaseSpellRecord : ActiveRecordBase<BaseSpellRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Breed")]
        public int Breed
        {
            get;
            set;
        }

        [Property("Level")]
        public int Level
        {
            get;
            set;
        }

        [Property("SpellID")]
        public int SpellID
        {
            get;
            set;
        }

        [Property("Position")]
        public int Position
        {
            get;
            set;
        }
    }
}
