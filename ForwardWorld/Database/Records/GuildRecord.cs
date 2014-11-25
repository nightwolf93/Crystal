using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("guilds_data")]
    public class GuildRecord : ActiveRecordBase<GuildRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "id")]
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

        [Property("emblem_backID")]
        public int EmblemBackID
        {
            get;
            set;
        }

        [Property("emblem_backColor")]
        public int EmblemBackColor
        {
            get;
            set;
        }

        [Property("emblem_frontID")]
        public int EmblemFrontID
        {
            get;
            set;
        }

        [Property("emblem_frontColor")]
        public int EmblemFrontColor
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

        [Property("experience")]
        public int Exp
        {
            get;
            set;
        }

        [Property("caract")]
        public string Caracts
        {
            get;
            set;
        }

        [Property("caract_points")]
        public int CaractPoints
        {
            get;
            set;
        }

        [Property("spell_points")]
        public int SpellPoints
        {
            get;
            set;
        }

        [Property("spells")]
        public string Spells
        {
            get;
            set;
        }
    }
}
