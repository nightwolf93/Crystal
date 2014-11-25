using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("guild_creator_location")]
    public class GuildCreatorLocationRecord : ActiveRecordBase<GuildCreatorLocationRecord>
    {
        [PrimaryKey("id")]
        public int ID
        {
            get;
            set;
        }

        [Property("mapid")]
        public int MapID
        {
            get;
            set;
        }

        [Property("cellid")]
        public int CellID
        {
            get;
            set;
        }

        [Property("level_required")]
        public int RequiredLevel
        {
            get;
            set;
        }
    }
}
