using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.RealmServer.AbstractClass;
using Crystal.RealmServer.Authentification.Manager;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Authentification.Network
{
    public partial class AuthentificationClient : AbstractClient
    {

        #region Fields

        public string EncrypKey;
        public AuthentificationHandler Handler;
        public AuthentificationState State = AuthentificationState.CheckVersion;
        public Database.Records.AccountRecord Account;

        #endregion

        public AuthentificationClient(SilverSock.SilverSocket socket)
            : base(socket)
        {
            this.Handler = new AuthentificationHandler(this);
            this.EncrypKey = Utilities.Basic.RandomString(32);
            Send("HC" + EncrypKey);
        }

        #region Overrides Methods

        public override void Disconnected()
        {
            try
            {
                lock (AuthentificationManager.Server.Clients)
                {
                    AuthentificationManager.Server.Clients.Remove(this);
                    if (this.Account != null)
                    {
                        this.Account.Logged = 0;
                        this.Account.SaveAndFlush();
                    }
                    Logger.LogInfo("Client disconnected !");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Can't disconnect player : " + e.ToString());
            }
        }

        public override void DataArrival(byte[] data)
        {
            try
            {
                string noParsedPacket = Encoding.ASCII.GetString(data);
                foreach (string packet in noParsedPacket.Replace("\x0a", "").Split('\x00'))
                {
                    if (packet == "")
                        continue;
                   // Logger.LogDebug("Received << " + packet);
                    Handler.Dispatch(packet);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        #endregion

    }
}
