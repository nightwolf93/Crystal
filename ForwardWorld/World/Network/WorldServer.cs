using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.AbstractClass;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public partial class WorldServer : AbstractServer
    {
        public List<WorldClient> Clients = new List<WorldClient>();

        public WorldServer(string adress, int port)
            : base(adress, port)
        {
            Start();
        }

        public override void ServerStarted()
        {
            Utilities.ConsoleStyle.Infos("WorldServer wait connection !");
        }

        public override void ServerFailed(Exception ex)
        {
            Utilities.ConsoleStyle.Error("WorldServer not running : " + ex.ToString());
        }

        public override void ServerAcceptClient(SilverSock.SilverSocket socket)
        {
            Utilities.ConsoleStyle.Infos("New input connection from realm !");
            this.Add(new WorldClient(socket));
        }

        public void Add(WorldClient client)
        {
            lock (Clients)
                Clients.Add(client);
        }

        public void Remove(WorldClient client)
        {
            lock (Clients)
                Clients.Remove(client);
        }
    }
}
