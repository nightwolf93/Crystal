using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.AbstractClass;

namespace Crystal.WorldServer.Communication.Rcon
{
    public class RConServer : AbstractServer
    {
        public List<RConClient> Clients = new List<RConClient>();

        public RConServer(string adress, int port)
            : base(adress, port)
        {
            Start();
        }

        public override void ServerStarted()
        {
            Utilities.ConsoleStyle.Infos("RConServer wait connection !");
        }

        public override void ServerFailed(Exception ex)
        {
            Utilities.ConsoleStyle.Error("RConServer not running : " + ex.ToString());
        }

        public override void ServerAcceptClient(SilverSock.SilverSocket socket)
        {
            Utilities.ConsoleStyle.Infos("New RConClient !");
            this.Clients.Add(new RConClient(socket));
        }
    }
}
