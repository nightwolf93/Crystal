using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.AbstractClass;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm
{
    public partial class CommunicationServer : AbstractServer
    {
        public RealmLink MainRealm;
        public List<RealmLink> NotVefifiedLink = new List<RealmLink>();

        public CommunicationServer(string adress, int port)
            : base(adress, port)
        {
            Start();
        }

        public override void ServerStarted()
        {
            Utilities.ConsoleStyle.Realm("CommunicationServer wait connection !");
        }

        public override void ServerFailed(Exception ex)
        {
            Utilities.ConsoleStyle.Error("CommunicationServer not running : " + ex.ToString());
        }

        public override void ServerAcceptClient(SilverSock.SilverSocket socket)
        {
            Utilities.ConsoleStyle.Realm("New realm connection !");
            NotVefifiedLink.Add(new RealmLink(socket));
        }
    }
}
