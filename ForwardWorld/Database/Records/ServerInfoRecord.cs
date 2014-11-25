using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("server_infos")]
    public class ServerInfoRecord : ActiveRecordBase<ServerInfoRecord>
    {
        [PrimaryKey("id")]
        public int ID
        {
            get;
            set;
        }

        [Property("uptime")]
        public string Uptime
        {
            get;
            set;
        }

        [Property("players_logged")]
        public int PlayersLogged
        {
            get;
            set;
        }
    }
}
