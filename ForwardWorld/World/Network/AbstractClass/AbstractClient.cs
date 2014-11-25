using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using SilverSock;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.AbstractClass
{
    public class AbstractClient
    {
        private SilverSocket _socket;
        public string IP { get { return this._socket.IP.Split(':')[0]; } }

        public AbstractClient(SilverSocket socket)
        {
            _socket = socket;
            _socket.OnDataArrivalEvent += new SilverEvents.DataArrival(DataArrival);
            _socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(Disconnected);
        }

        public void Send(string data)
        {
            try
            {
                data = Utilities.Basic.MakeAccent(data);
                if (Program.DebugMode) Utilities.ConsoleStyle.Debug("Sended >> " + data);
                byte[] packet = System.Text.Encoding.Default.GetBytes(data + "\x00");
                _socket.Send(packet);
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Erreur : " + e.Message);
            }

        }

        public void Send(byte[] data)
        {
            _socket.Send(data);
        }

        public void Close()
        {
            _socket.CloseSocket();
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
