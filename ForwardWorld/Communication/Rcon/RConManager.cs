using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Communication.Rcon
{
    public static class RConManager
    {
        public static RConServer Server { get; set; }

        public static void InitServer()
        {
            Server = new RConServer(Utilities.ConfigurationManager.GetStringValue("RConHost"), Utilities.ConfigurationManager.GetIntValue("RConPort"));
        }
    }
}
