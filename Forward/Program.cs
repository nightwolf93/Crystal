using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project Forward

namespace Crystal.RealmServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.Title = "Crystal RealmServer";

                Logger.LogInfo("Start the connection with the database ...");
                Utilities.ConfigurationManager.LoadConfiguration();
                Database.Manager.DatabaseManager.StartDatabase();
                Logger.LogInfo("Connected to the database !");

                if (Utilities.ConfigurationManager.GetBoolValue("CreateShema"))
                {
                    Logger.LogInfo("Create database shema ...");
                    Database.Manager.DatabaseManager.InitTable();
                    Logger.LogInfo("Shema created !");
                }

                Logger.LogInfo("Create cache ...");
                Database.Cache.GameServerCache.Init();
                Database.Cache.AccountCharactersInformationsCache.Init();
                Logger.LogInfo("Cache created !");

                Authentification.Helper.AuthentificationHelper.ResetLogged();
                Logger.LogInfo("Reset logged accounts state !");

                Authentification.Network.AuthentificationQueue.Start();
                Authentification.Manager.AuthentificationManager.LaunchService();
                Communication.World.Manager.WorldCommunicator.InitConnections();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
