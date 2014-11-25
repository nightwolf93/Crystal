using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SilverSock;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.AbstractClass
{
    public abstract class AbstractServer
    {
        private string _adress;
        private int _port;
        private SilverServer _server;

        public AbstractServer(string adress, int port)
        {
            _adress = adress;
            _port = port;
            _server = new SilverServer(_adress, _port);
        }

        public void Start()
        {
            if (_server != null)
            {
                _server.OnListeningEvent += new SilverEvents.Listening(ServerStarted);
                _server.OnListeningFailedEvent += new SilverEvents.ListeningFailed(ServerFailed);
                _server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(ServerAcceptClient);
                _server.WaitConnection();
            }
        }

        public virtual void ServerStarted()
        {
            throw new NotImplementedException();
        }

        public virtual void ServerFailed(Exception ex)
        {
            throw new NotImplementedException();
        }

        public virtual void ServerAcceptClient(SilverSocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
