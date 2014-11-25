using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Zivsoft.Log;

using Crystal.WorldServer.Utilities;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : KolizeumManager
*/

namespace Crystal.WorldServer.World.Game.Kolizeum
{
    public static class KolizeumManager
    {
        public static System.Timers.Timer KoliCheckTimer { get; set; }

        public static List<Network.WorldClient> RegisteredClient = new List<Network.WorldClient>();
        public static List<KolizeumMap> Maps = new List<KolizeumMap>();

        public static void LaunchKolizeumTask()
        {
            try
            {
                if (ConfigurationManager.GetBoolValue("EnableKolizeum"))
                {
                    KoliCheckTimer = new System.Timers.Timer(ConfigurationManager.GetIntValue("KolizeumCheckIntervall"));
                    KoliCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(KoliCheckTimer_Elapsed);
                    KoliCheckTimer.Start();
                    Utilities.ConsoleStyle.Infos("Kolizeum task started !");
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't start kolizeum : " + e.ToString());
            }
        }

        public static void LoadMaps()
        {
            var reader = new StreamReader("Datas/Kolizeum.txt");
            var maps = reader.ReadToEnd();
            reader.Close();
            foreach (var map in maps.Split('|'))
            {
                if (map != "")
                {
                    var data = map.Split(';');
                    var mapid = int.Parse(data[0]);
                    var cellid = int.Parse(data[0]);
                    Maps.Add(new KolizeumMap() { MapID = mapid, CellID = cellid });
                }
            }
        }

        public static bool IsRegistered(World.Network.WorldClient client)
        {
            return RegisteredClient.Contains(client);
        }

        public static void SubscribeToKolizeum(Network.WorldClient client)
        {
            lock (RegisteredClient)
            {
                if (!RegisteredClient.Contains(client))
                {
                    RegisteredClient.Add(client);
                    client.Action.SystemMessage("Vous etes desormais <b>inscrit</b> au kolizeum, patienter pour avoir un match !");
                }
                else
                {
                    client.Action.SystemMessage("Vous etes <b>deja inscrit</b> au kolizeum");
                }
            }
        }

        public static void UnSubscribeToKolizeum(Network.WorldClient client)
        {
            lock (RegisteredClient)
            {
                if (RegisteredClient.Contains(client))
                {
                    RegisteredClient.Remove(client);
                    client.Action.SystemMessage("Vous n'etes desormais <b>pu inscrit</b> au kolizeum !");
                }
                else
                {
                    client.Action.SystemMessage("Vous <b>n'etes pas inscrit</b> au kolizeum, faite .koli on");
                }
            }
        }

        private static void KoliCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (RegisteredClient)
            {
                try
                {
                    lock (RegisteredClient)
                    {
                        MakeTeams();
                    }
                }
                catch (Exception ex)
                {
                    Utilities.ConsoleStyle.Error("Can't execute kolizeum task : " + ex.ToString());
                }
            }
        }

        private static KolizeumMap RandomMap()
        {
            return Maps[Utilities.Basic.Rand(0, Maps.Count - 1)];
        }

        private static void MakeTeams()
        {
            var tempList = new List<KolizeumTeam>();
            var currentKoliTeam = new KolizeumTeam();
            foreach (var client in RegisteredClient.ToArray())
            {
                try
                {
                    client.Action.KolizeumMessage("Preparation des equipes, merci de patienter ...");
                    if (!client.Action.IsOccuped)
                    {
                        currentKoliTeam.AddMember(client);
                        if (currentKoliTeam.IsFull)
                        {
                            tempList.Add(currentKoliTeam);
                            currentKoliTeam = new KolizeumTeam();
                        }
                    }
                    else
                    {
                        client.Action.KolizeumMessage("Vous etes occuper, votre participation au kolizeum a ete annuler !");
                        UnSubscribeToKolizeum(client);
                    }
                }
                catch (Exception e)
                {
                    client.Action.KolizeumMessage("Votre participation au kolizeum a ete annuler du a une erreur lie a votre personnage");
                }
            }

            //Get formatted teams
            for (int i = 0; i <= tempList.Count - 1; i += 2)
            {
                try
                {
                    var redTeam = tempList[i];
                    var blueTeam = tempList[i + 1];
                    redTeam.UnsubcribeMembers();
                    blueTeam.UnsubcribeMembers();
                    var match = new KolizeumMatch(redTeam, blueTeam);
                    match.Map = RandomMap();
                    match.InitializeMatch();
                }
                catch (Exception e)
                {
                    //Utilities.ConsoleStyle.Error("Can't make matchmaking : " + e.ToString());
                }
            }
        }
    }
}
