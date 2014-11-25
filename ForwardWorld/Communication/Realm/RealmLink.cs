using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm
{
    public class RealmLink : AbstractClass.AbstractClient
    {
        public bool IsMain = false;
        public object PacketLock = new object();

        public RealmLink(SilverSock.SilverSocket socket)
            : base(socket)
        {
            System.Threading.Thread.Sleep(500);
            SendMessage(new Packet.HelloKeyMessage());
        }

        public override void Disconnected()
        {
            if (IsMain)
            {
                Communicator.Server.MainRealm = null;
            }
            Communicator.Server.NotVefifiedLink.Remove(this);
            Utilities.ConsoleStyle.Error("Realm disconnected !");
        }

        public override void DataArrival(byte[] data)
        {
            Protocol.ForwardPacket packet = new Protocol.ForwardPacket(data);
            //Logger.LogDebug("Received " + packet.ID.ToString() + " from realm (lenght : " + packet.GetBytes.Length + ")");
            Dispatch(packet);
        }

        public void SendMessage(Protocol.ForwardPacket packet)
        {
            try
            {
                //Logger.LogDebug("Send " + packet.ID.ToString() + " to realm (lenght : " + packet.GetBytes.Length + ")");
                Send(packet.GetBytes);
            }
            catch
            {
                Utilities.ConsoleStyle.Error("Can't send packet to realm");
            }
        }

        public void Dispatch(Protocol.ForwardPacket packet)
        {
            try
            {
                switch (packet.ID)
                {
                    case Protocol.ForwardPacketTypeEnum.SecureKeyMessage:
                        Communicator.ReceivedKey(this, packet);
                        break;

                    case Protocol.ForwardPacketTypeEnum.PlayerCommingMessage:
                        Communicator.ReceivedPlayer(this, packet);
                        break;

                    case Protocol.ForwardPacketTypeEnum.KickPlayerMessage:
                        Communicator.ReceivedKickPlayer(this, packet);
                        break;
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Error : " + e.ToString());
            }
        }
    }
}
