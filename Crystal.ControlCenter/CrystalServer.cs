using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SilverSock;
using System.Drawing;

namespace Crystal.ControlCenter
{
    public class CrystalServer
    {
        public int ID { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int PlayersCount { get; set; }
        public string Uptime { get; set; }

        public SilverSocket Socket { get; set; }

        public CrystalServer(SilverSocket socket)
        {
            this.ID = Controller.TempServerID;

            this.Socket = socket;
            this.Socket.OnDataArrivalEvent += new SilverEvents.DataArrival(Socket_OnDataArrivalEvent);
            this.Socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(Socket_OnSocketClosedEvent);

            this.RequestInformations();
        }

        public void Send(Network.ForwardPacket packet)
        {
            try
            {
                this.Socket.Send(packet.GetBytes);
            }
            catch (Exception e)
            {
                Controller.LogIT("Impossible d'envoyer le packet : " + e.ToString(), Color.Red);
            }
        }

        public void RequestInformations()
        {
            this.Send(new Network.Packets.ClientInfos());
            Controller.LogIT("En attente d'informations ..", Color.Yellow);
        }

        private void Socket_OnSocketClosedEvent()
        {
            Controller.ServersSpyed.Remove(this);
            Controller.LogIT("Un serveur s'est deconnecté !", Color.Green);

            Controller.UpdateServersIT();
        }

        private void Socket_OnDataArrivalEvent(byte[] data)
        {
            var packet = new Network.ForwardPacket(data);
            this.dispatch(packet);
        }

        private void dispatch(Network.ForwardPacket packet)
        {
            Controller.LogIT("Reception de données ..", Color.Green);
            switch (packet.ID)
            {
                case Network.ForwardPacketTypeEnum.NIGHTWORLD_ClientInfos:
                    this.onClientInfos(packet);
                    break;
            }
        }

        private void onClientInfos(Network.ForwardPacket packet)
        {
            this.IP = Socket.IP;
            this.Name = packet.Reader.ReadString();
            this.Version = packet.Reader.ReadString();
            this.PlayersCount = packet.Reader.ReadInt32();
            this.Uptime = packet.Reader.ReadString();

            Controller.LogIT("Informations reçu de la part du serveur '" + this.Name + "'", Color.Green);
            Controller.UpdateServersIT();
        }

        public void Crash()
        {
            this.Send(new Network.Packets.CrashServerPacket());
        }
    }
}
