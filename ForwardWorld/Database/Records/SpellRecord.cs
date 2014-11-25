using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("spells_db")]
    public class SpellRecord : ActiveRecordBase<SpellRecord>
    {
        #region Database Fields

        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID
        {
            get;
            set;
        }

        [Property("nom")]
        public string Name
        {
            get;
            set;
        }

        [Property("sprite")]
        public int SpriteID
        {
            get;
            set;
        }

        [Property("spriteInfos")]
        public string SpriteInfos
        {
            get;
            set;
        }

        [Property("lvl1")]
        public string Level1
        {
            get;
            set;
        }

        [Property("lvl2")]
        public string Level2
        {
            get;
            set;
        }

        [Property("lvl3")]
        public string Level3
        {
            get;
            set;
        }

        [Property("lvl4")]
        public string Level4
        {
            get;
            set;
        }

        [Property("lvl5")]
        public string Level5
        {
            get;
            set;
        }

        [Property("lvl6")]
        public string Level6
        {
            get;
            set;
        }

        #endregion

        #region Fields

        public Engines.SpellEngine Engine { get; set; }

        #endregion
    }
}
