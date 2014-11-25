using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Communication.World.Network
{
    public class WorldLink
    {
        private SilverSock.SilverSocket socket;
        public Helper.WorldState State = Helper.WorldState.Online;
        public Database.Records.GameServerRecord GameServer;
        public List<string> ConnectedAccount = new List<string>();

        public static object PacketLock = new object();

        private Timer retryTimer = new Timer(5000);

        public WorldLink(Database.Records.GameServerRecord GameServer)
        {
            this.GameServer = GameServer;
            socket = new SilverSock.SilverSocket();
            socket.OnConnected += new SilverSock.SilverEvents.Connected(Connected);
            socket.OnDataArrivalEvent += new SilverSock.SilverEvents.DataArrival(DataArrival);
            socket.OnFailedToConnect += new SilverSock.SilverEvents.FailedToConnect(FailedToConnect);
            socket.OnSocketClosedEvent += new SilverSock.SilverEvents.SocketClosed(LostConnection);
            retryTimer.Elapsed += new ElapsedEventHandler(RetryToConnect);
            System.Threading.Thread.Sleep(150);
            socket.ConnectTo(GameServer.Adress, GameServer.CommunicationPort);
        }

        #region Events

        private void RetryToConnect(object sender, ElapsedEventArgs e)
        {
            socket.ConnectTo(GameServer.Adress, GameServer.CommunicationPort);
        }

        private void LostConnection()
        {
            State = Helper.WorldState.Offline;
            Logger.LogError("Lost connection with the worldserver '" + GameServer.ID + "'");
            retryTimer.Enabled = true;
            retryTimer.Start();
            Manager.WorldCommunicator.RefreshServers();
            ConnectedAccount.Clear();
        }

        private void FailedToConnect(Exception ex)
        {
            State = Helper.WorldState.Offline;
            Logger.LogError("Failed to connect on worldserver '" + GameServer.ID + "'");
            retryTimer.Enabled = true;
            retryTimer.Start();
            ConnectedAccount.Clear();
        }

        private void DataArrival(byte[] data)
        {
            try
            {
                Protocol.ForwardPacket packet = new Protocol.ForwardPacket(data);
                Logger.LogDebug("Received packet " + packet.ID.ToString() + " from worldserver (lenght : " + packet.Reader.BaseStream.Length + ")");
                Dispatch(packet);
            }
            catch (Exception e)
            {
                Logger.LogError("Error : " + e.ToString());
            }
        }

        private void Connected()
        {
            State = Helper.WorldState.Online;
            Logger.LogInfo("Connected to worldserver '" + GameServer.ID + "'");
            retryTimer.Enabled = false;
            retryTimer.Close();
            Manager.WorldCommunicator.RefreshServers();
        }

        #endregion

        #region Handler

        private void Dispatch(Protocol.ForwardPacket packet)
        {
            switch (packet.ID)
            {
                case Protocol.ForwardPacketTypeEnum.HelloKeyMessage:
                    Manager.WorldCommunicator.SendSecureKey(this);
                    break;

                case Protocol.ForwardPacketTypeEnum.PlayerCreatedCharacterMessage:
                    Manager.WorldCommunicator.ReceivedCharacterCreated(this, packet);
                    break;

                case Protocol.ForwardPacketTypeEnum.PlayerDeletedCharacterMessage:
                    Manager.WorldCommunicator.ReceivedCharacterDeleted(this, packet);
                    break;

                case Protocol.ForwardPacketTypeEnum.WorldSave:
                    State = Helper.WorldState.InSave;
                    Manager.WorldCommunicator.RefreshServers();
                    break;

                case Protocol.ForwardPacketTypeEnum.WorldSaveFinished:
                    State = Helper.WorldState.Online;
                    Manager.WorldCommunicator.RefreshServers();
                    break;

                case Protocol.ForwardPacketTypeEnum.WorldMaintenance:
                    State = Helper.WorldState.InMaintenance;
                    Manager.WorldCommunicator.RefreshServers();
                    break;

                case Protocol.ForwardPacketTypeEnum.WorldMaintenanceFinished:
                    State = Helper.WorldState.Online;
                    Manager.WorldCommunicator.RefreshServers();
                    break;

                case Protocol.ForwardPacketTypeEnum.PlayerConnectedMessage:
                    Manager.WorldCommunicator.ReceivedPlayerConnected(this, packet);
                    break;

                case Protocol.ForwardPacketTypeEnum.PlayerDisconnectedMessage:
                    Manager.WorldCommunicator.ReceivedPlayerDisconnected(this, packet);
                    break;

                case Protocol.ForwardPacketTypeEnum.ClientShopPointUpdateMessage:
                    Manager.WorldCommunicator.ReceivedShopPointUpdate(this, packet);
                    break;
            }
        }

        #endregion

        #region Methods

        public void SendMessage(Protocol.ForwardPacket packet)
        {
            try
            {
                lock (PacketLock)
                {
                    Logger.LogDebug("Send packet " + packet.ID.ToString() + " to worldserver (lenght : " + packet.GetBytes.Length + ")");
                    socket.Send(packet.GetBytes);
                }

            }
            catch { }
        }

        #endregion

    }
}
