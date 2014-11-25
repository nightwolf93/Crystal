using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zivsoft.Log;

using Crystal.RealmServer.AbstractClass;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Authentification.Network
{
    public partial class AuthentificationServer : AbstractServer
    {
        public List<AuthentificationClient> Clients = new List<AuthentificationClient>();

        public AuthentificationServer(string adress, int port)
            : base(adress, port)
        {
            Start();
        }

        public override void ServerStarted()
        {
            Logger.LogInfo("AuthentificationServer wait connection");
        }

        public override void ServerFailed(Exception ex)
        {
            Logger.LogError("AuthentificationServer has failed to open : " + ex.ToString());
        }

        public override void ServerAcceptClient(SilverSock.SilverSocket socket)
        {
            try
            {
                Logger.LogInfo("New input connection !" + socket.IP);
                lock(Clients)
                    Clients.Add(new AuthentificationClient(socket));
            }
            catch (Exception e)
            {
                Logger.LogError("Can't accept connection : " + e.ToString());
            }
        }
    }
}
