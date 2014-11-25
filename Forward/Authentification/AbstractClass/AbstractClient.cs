using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using SilverSock;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.AbstractClass
{
    public class AbstractClient
    {
        private SilverSocket _socket;

        public AbstractClient(SilverSocket socket)
        {
            try
            {
                _socket = socket;
                _socket.OnDataArrivalEvent += new SilverEvents.DataArrival(DataArrival);
                _socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(Disconnected);
            }
            catch (Exception e)
            {
                Logger.LogError("Can't create abstract client : " + e.ToString());
            }
        }

        public void Send(string data)
        {
            try
            {
                //Logger.LogDebug("Sended >> " + data);
                byte[] packet = Encoding.ASCII.GetBytes(data + "\x00");
                _socket.Send(packet);
            }
            catch (Exception e)
            {
                Logger.LogError("Can't send packet : " + e.ToString());
            }
        }

        public void Close()
        {
            try
            {
                _socket.CloseSocket();
            }
            catch (Exception e)
            {
                Logger.LogError("Can't close connection : " + e.ToString());
            }
        }

        public virtual void DataArrival(byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual void Disconnected()
        {
            throw new NotImplementedException();
        }
    }
}
