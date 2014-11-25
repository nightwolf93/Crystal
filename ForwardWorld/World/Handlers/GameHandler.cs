using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

using Crystal.WorldServer.Utilities;
using Crystal.WorldServer.World;
using Crystal.WorldServer.World.Network;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class GameHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("GC", typeof(GameHandler).GetMethod("CreateContext"));
            Network.Dispatcher.RegisteredMethods.Add("GI", typeof(GameHandler).GetMethod("GameInformationsRequest"));
            Network.Dispatcher.RegisteredMethods.Add("GA", typeof(GameHandler).GetMethod("GameAction"));
            Network.Dispatcher.RegisteredMethods.Add("GKE", typeof(GameHandler).GetMethod("RequestChangePath"));
            Network.Dispatcher.RegisteredMethods.Add("GKK", typeof(GameHandler).GetMethod("EndAction"));
            Network.Dispatcher.RegisteredMethods.Add("GP", typeof(GameHandler).GetMethod("RequestChangeFactionState"));
            Network.Dispatcher.RegisteredMethods.Add("fL", typeof(GameHandler).GetMethod("RequestFightsList"));
            Network.Dispatcher.RegisteredMethods.Add("fD", typeof(GameHandler).GetMethod("RequestFightsDetails"));
            Network.Dispatcher.RegisteredMethods.Add("eD", typeof(GameHandler).GetMethod("RequestChangeOrientation"));
        }

        public static void GameAction(WorldClient client, string packet)
        {
            int Type = int.Parse(packet.Substring(2, 3));
            string Parameters = packet.Substring(5);
            switch (Type)
            {
                case 1:
                    RequestMove(client, Parameters);
                    break;

                case 300://Cast spell
                    RequestCastSpell(client, Parameters);
                    break;

                case 303://Weapon
                    RequestUseWeapon(client, Parameters);
                    break;

                case 500://IO Object
                    ParseIOObjects(client, Parameters);
                    break;

                case 900:
                    RequestChallenge(client, Parameters);
                    break;

                case 901:
                    AcceptChallengeRequest(client);
                    break;

                case 902:
                    EndChallengeRequest(client);
                    break;

                case 903:
                    RequestJoinChallenge(client, Parameters);
                    break;

                case 906:
                    RequestAggresion(client, Parameters);
                    break;
            }
        }

        public static void ParseIOObjects(WorldClient client, string IO)
        {
            string[] data = IO.Split(';');
            if (client.Character.GetJobBySkill(int.Parse(data[1])) != null)
            {
                var job = client.Character.GetJobBySkill(int.Parse(data[1]));
                var skill = job.GetSkill(int.Parse(data[1]));
                var io = client.Character.Map.Engine.GetIO(int.Parse(data[0]));
                if (io != null)
                {
                    if (client.Action.NextJobSkill == null)
                    {
                        client.Action.NextJobSkill = skill;
                        return;
                    }
                }
            }
            switch ((Enums.InteractiveObjectEnum)int.Parse(data[1]))
            {
                case Enums.InteractiveObjectEnum.SAVE_POSITION:
                    ZaapHandler.SavePosition(client, client.Character.MapID);
                    break;

                case Enums.InteractiveObjectEnum.ZAAP:
                    ZaapHandler.OnRequestUse(client, int.Parse(data[0]));
                    break;

                case Enums.InteractiveObjectEnum.INCARNAM_STATUS:
                    client.State = WorldClientState.OnRequestIncarnamStatue;
                    break;

                case Enums.InteractiveObjectEnum.MOUNT_DOOR:
                    client.State = WorldClientState.OnRequestMountDoor;
                    break;
            }
        }

        public static void RequestChangePath(WorldClient client, string packet)
        {
            client.Character.CellID = int.Parse(packet.Substring(5));
        }

        public static void RequestMove(WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                client.Character.Fighter.Team.Fight.PlayerWantMove(client.Character.Fighter, packet);
                return;
            }
            if (!client.Action.IsOverPod)
            {
                client.Action.EndAutoRegen();
                Engines.Pathfinding pathfinding = new Engines.Pathfinding(packet, client.Character.Map,
                                                                        client.Character.CellID, client.Character.Direction);

                int dTest = pathfinding.GetDistanceBetween(client.Character.CellID, pathfinding.Destination);
                string remakePath = pathfinding.GetStartPath + pathfinding.RemakePath();
                client.Character.Direction = pathfinding.NewDirection;
                client.Character.NextMove = pathfinding.Destination;

                client.Character.Map.Engine.Send("GA0;1;" + client.Character.ID + ";" + remakePath);

                client.State = WorldClientState.OnMove;
            }
            else
            {
                client.SendImPacket("112");
                client.Send("GA;0");
            }
        }

        public static void EndAction(WorldClient client, string packet)
        {
            if (packet[3] == '0')
            {
                #region Battle

                /* If in battle */
                if (client.Character.Fighter != null)
                {
                    client.Character.Fighter.Team.Fight.PlayerEndMove(client.Character.Fighter);
                    return;
                }

                #endregion

                #region Cell ID

                var oldCell = client.Character.CellID;
                client.Character.CellID = client.Character.NextMove;

                #endregion

                #region Dropped Items

                Database.Records.WorldItemRecord droppedItem = client.Character.Map.Engine.GetDroppedItem(client.Character.CellID);
                if (droppedItem != null)
                {
                    if (client.Character.Items.HaveItemWithSameEffects(droppedItem.Effects) &&
                        client.Character.Items.HaveItem(droppedItem.Template))
                    {
                        client.Character.Items.AddItem(droppedItem, false, droppedItem.Quantity);
                    }
                    else
                    {
                        client.Character.Items.AddItem(droppedItem, true, droppedItem.Quantity);
                        droppedItem.Owner = client.Character.ID;
                    }
                    client.Character.Map.Engine.RemoveDroppedItem(droppedItem, client.Character.CellID);
                }

                #endregion

                #region Zaaps

                /* Zaap use request */
                if (client.State == WorldClientState.OnRequestZaap)
                {
                    if (client.Character.Map.Engine.Zaap != null)
                    {
                        if (client.Character.CellID == client.Character.Map.Engine.Zaap.CellID)
                        {
                            ZaapHandler.OpenZaapPanel(client);
                        }
                    }
                }

                #endregion

                #region Incarnam Teleporter

                /* Incarnam statue teleporter request */
                if (client.State == WorldClientState.OnRequestIncarnamStatue)
                {
                    Database.Records.IncarnamTeleportRecord incarnamTP = Helper.MapHelper.FindIncarnamTeleporter(client.Character.MapID);
                    if (incarnamTP != null)
                    {
                        if (incarnamTP.CellID == client.Character.CellID)
                        {
                            if (incarnamTP.MaxLevel > client.Character.Level)
                            {
                                Database.Records.OriginalBreedStartMapRecord startmap = Helper.MapHelper.GetOriginalBreedStartMap(client.Character.Breed);
                                Network.World.GoToMap(client, startmap.MapID, startmap.CellID);
                            }
                            else
                            {
                                client.SendImPacket("13");
                            }
                            client.State = WorldClientState.None;
                        }
                    }
                }

                #endregion

                #region Guild Creator Location

                /* Guid creator location */
                if (Utilities.ConfigurationManager.GetBoolValue("EnableGuildCreationLocation"))
                {
                    Database.Records.GuildCreatorLocationRecord guildCreator = Helper.MapHelper.FindGuildCreator(client.Character.MapID, client.Character.CellID);
                    if (guildCreator != null)
                    {
                        if (client.Character.Level >= guildCreator.RequiredLevel)
                        {
                            //TODO: Check object creator required
                            client.Send("gn");
                        }
                        else
                        {
                            client.SendImPacket("13");
                        }
                    }
                }

                #endregion

                #region Mount Door

                if (client.State == WorldClientState.OnRequestMountDoor)
                {
                    client.Character.Map.Engine.ShowPaddocksMounts(client, client.Character.CellID);
                }

                #endregion

                #region Job

                if (client.Action.NextJobSkill != null)
                {
                    var io = client.Character.Map.Engine.GetIO(client.Character.CellID);
                    if (io != null)
                    {
                        if (!client.Action.NextJobSkill.DoSkill(client, io))
                        {
                            client.Action.NextJobSkill = null;
                        }
                    }
                    else
                    {
                        client.Action.NextJobSkill = null;
                    }
                }

                #endregion

                #region Monsters

                /* Monsters On Pos */
                Engines.Map.MonsterGroup monstersOnCell = client.Character.Map.Engine.GetMonsterGroupOnCell(client.Character.CellID);
                if (monstersOnCell != null)
                {
                    if (monstersOnCell.Leader == null)
                    {
                        return;
                    }
                    var freeCell = client.Character.Map.Engine.PathfindingMaker.FreeCellNeightboor(client.Character.CellID);
                    if (freeCell != -1)
                    {
                        client.Character.CellID = freeCell;
                    }
                    StartMonstersBattle(client, monstersOnCell);
                    client.Character.Map.Engine.RemoveMonstersOnMap(monstersOnCell);
                    client.Character.Map.Engine.Spawner.GenerateOneGroup();
                    client.Character.Map.Engine.Players.CharactersOnMap.ForEach(x => client.Character.Map.Engine.ShowMonstersGroup(x));
                    return;
                }

                #endregion

                #region Triggers

                /* Change map by trigger */
                if (client.Character.Map.Triggers.FindAll(x => x.CellID == client.Character.CellID).Count > 0)
                {
                    Database.Records.TriggerRecord trigger = client.Character.Map.Triggers.FirstOrDefault(x => x.CellID == client.Character.CellID);
                    if (client.Character.Level >= trigger.LevelRequired)
                    {
                        World.Network.World.GoToMap(client, trigger.NextMap, trigger.NextCell);
                    }
                    else
                    {
                        client.Action.SystemMessage("Vous n'avez pas level requis pour rentrer sur cette carte, level requis : <b>" + trigger.LevelRequired + "</b>");
                    }
                    return;
                }

                #endregion

                #region Script

                Interop.PythonScripting.ScriptManager.CallEventPlayerMovement(client, oldCell, client.Character.CellID);

                #endregion
            }
            else
            {
                //Doing job stuff
                if (client.Action.NextJobSkill != null)
                {
                    var io = client.Character.Map.Engine.GetIO(client.Character.CellID);
                    if (io != null)
                    {
                        client.Action.NextJobSkill.SkillFinished(client, io);
                    }
                    client.Action.NextJobSkill = null;
                }
            }

            client.State = WorldClientState.None;
        }

        public static void CreateContext(WorldClient client, string packet)
        {
            int contextType = int.Parse(packet.Substring(2));
            switch (contextType)
            {
                case 1://Roleplay
                    client.Send("GCK|1|" + client.Character.Nickname);
                    client.Send("cC+*#$pi:?%");
                    client.Send("AR6bk");
                    if (client.State == WorldClientState.SelectCharacter)
                    {
                        World.Network.World.GoToMap(client, client.Character.Map, client.Character.CellID, true);
                        if (ConfigurationManager.GetBoolValue("ShowOfficialDofusMessage"))
                        {
                            client.SendImPacket("189");
                        }
                        if (client.Character.MapID == ConfigurationManager.GetIntValue("StartMap"))
                        {
                            if (ConfigurationManager.GetBoolValue("ShowIncarnamMultiInstanceMessage"))
                            {
                                client.SendImPacket("1183");
                            }
                        }
                        client.SendImPacket("0152", "?~?~?~?~?~127.0.0.1");//TODO! Last connection message
                        if (ConfigurationManager.GetBoolValue("WelcomeMessageEnabled"))
                        {
                            client.Action.BasicMessage(ConfigurationManager.GetStringValue("WelcomeMessage"),
                                                            ConfigurationManager.GetStringValue("WelcomeMessageColor"));
                        }
                        if(ConfigurationManager.GetBoolValue("ShowPlayersConnection"))
                        {
                            Manager.WorldManager.SendMessage("Le joueur <b>" + client.Character.Nickname + "</b> viens de se connecter !");
                        }
                        client.State = WorldClientState.None;
                        client.Character.RefreshItemSet();
                        client.Character.Stats.RefreshStats();
                        client.Action.RefreshCharacterJob();
                        client.Action.RefreshPods();
                        FriendHandler.WarnConnectionToFriends(client);
                        client.Send("SLo+");
                        client.Character.Spells.SendSpells();

                        /* Remove all actions */
                        client.Action.Spectator = null;
                        client.Character.Fighter = null;
                        
                        /* Send guild if have one */
                        if (client.Action.GuildMember != null && client.Action.Guild != null)
                        {
                            client.Action.GuildMember.SetOnline(client);
                            client.Action.Guild.SendGuildBasicInformation(client);
                        }

                        if (client.Account.AdminLevel > 0)
                        {
                            World.Network.World.SendNotification("Le membre du staff '" + client.Character.Nickname + "' est desormais en ligne");
                        }

                        client.Action.SendMountPanel();

                        //Game trailer
                        if (client.Character.FirstPlay)
                        {
                            client.Send("TB");
                            client.Character.FirstPlay = false;
                        }
                    }
                    else
                    {
                        World.Network.World.GoToMap(client, client.Character.Map, client.Character.CellID);
                        client.Character.Stats.ResetBonus();
                        client.Character.Stats.RefreshStats();
                        client.Character.Fighter = null;
                    }
                    break;
            }
        }

        public static void GameInformationsRequest(WorldClient client, string packet)
        {
            Utilities.ConsoleStyle.Debug("Request map details ..");
            /* Display all contents */
            try
            {
                client.Character.Map.Engine.Players.ShowPlayers(client);
                client.Character.Map.Engine.ShowNpcsOnMap(client);
                client.Character.Map.Engine.ShowMonstersGroup(client);
                client.Character.Map.Engine.ShowAllFightOnMap(client);
                client.Character.Map.Engine.ShowAllDroppedItems(client);
                client.Character.Map.Engine.ShowPaddocks(client);
                client.Character.Map.Engine.ShowIO(client);
                client.Send("GDK");

                /* Save zaaps */
                Database.Records.ZaapRecord zaap = Helper.ZaapHelper.GetZaap(client.Character.Map.ID);
                if (zaap != null)
                {
                    if (!client.Character.Zaaps.Contains(zaap.MapID))
                    {
                        client.Character.Zaaps.Add(zaap.MapID);
                        client.SendImPacket("024");
                    }
                }

            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't show informations on map : " + e.ToString());
                client.Send("GDK");
            }

            try
            {
                Interop.PythonScripting.ScriptManager.CallEventEnterMap(client, client.Character.MapID, client.Character.CellID);
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't execute script on enter map : " + e.ToString());
            }
        }

        public static void RequestChallenge(WorldClient client, string packet)
        {
            if (!client.Action.IsOccuped)
            {
                Network.WorldClient challenged = client.Character.Map.Engine.GetClientOnMap(int.Parse(packet));
                if (challenged != null)
                {
                    if (!challenged.Action.IsOccuped)
                    {
                        if (Utilities.ConfigurationManager.GetBoolValue("EnableMultiIPSecurity"))
                        {
                            if (client.IP == challenged.IP)
                            {
                                client.Action.SystemMessage("Impossible vous défier vous même !");
                                return;
                            }
                        }
                        client.State = WorldClientState.OnRequestChallenge;
                        client.Action.RequestChallengerID = challenged.Character.ID;
                        challenged.Action.RequestChallengerID = client.Character.ID;
                        client.Character.Map.Engine.Send("GA;900;" + client.Character.ID + ";" + challenged.Character.ID);
                    }
                    else
                    {
                        client.SendImPacket("114", challenged.Character.Nickname);
                    }
                }
            }
            else
            {
                client.Action.SystemMessage("Vous etes occuper !");
            }
        }

        public static void EndChallengeRequest(WorldClient client)
        {
            if(client.Action.RequestChallengerID == -1)
                return;

            WorldClient otherClient = Helper.WorldHelper.GetClientByCharacter(client.Action.RequestChallengerID);
            if (otherClient != null)
            {
                otherClient.State = WorldClientState.None;
                otherClient.Action.RequestChallengerID = -1;
                otherClient.Send("GA;902;" + otherClient.Character.ID + ";" + client.Character.ID);
            }
            client.State = WorldClientState.None;
            client.Action.RequestChallengerID = -1;
            otherClient.Send("GA;902;" + client.Character.ID + ";" + otherClient.Character.ID);           
        }

        public static void AcceptChallengeRequest(WorldClient client)
        {
            if (client.Action.RequestChallengerID != -1)
            {
                if (client.Character.Fighter != null)
                {
                    return;
                }
                Network.WorldClient otherClient = client.Character.Map.Engine.GetClientOnMap(client.Action.RequestChallengerID);
                if (otherClient != null)
                {
                    if (otherClient.Character.Fighter != null)
                    {
                        return;
                    }
                    client.Action.EndAutoRegen();
                    otherClient.Action.EndAutoRegen();
                    client.Send("GA;901;" + client.Character.ID + ";" + otherClient.Character.ID);
                    otherClient.Send("GA;901;" + otherClient.Character.ID + ";" + client.Character.ID);

                    /* Reset state */
                    client.Action.RequestChallengerID = -1;
                    otherClient.Action.RequestChallengerID = -1;

                    /* Starting battle engine */
                    Game.Fights.Fight fight = new Game.Fights.Fight(new Game.Fights.Fighter(client),
                                                                    new Game.Fights.Fighter(otherClient), 
                                                                    client.Character.Map.Engine,
                                                                    Enums.FightTypeEnum.Challenge);

                    client.Character.Map.Engine.AddFightOnMap(fight);

                    /* Destroy on the map */
                    client.Character.Map.Engine.RemovePlayer(client);
                    otherClient.Character.Map.Engine.RemovePlayer(otherClient);

                    /* Enable fight context */
                    fight.EnableContext();
                }
                else
                {
                    EndChallengeRequest(client);
                }
            }
        }

        public static void StartMonstersBattle(WorldClient client, Engines.Map.MonsterGroup group)
        {
            try
            {
                if (client.Character.Fighter != null)
                {
                    return;
                }
                client.Action.EndAutoRegen();
                Game.Fights.Fight fight = new Game.Fights.Fight(new Game.Fights.Fighter(client),
                                    group,
                                    client.Character.Map.Engine,
                                    Enums.FightTypeEnum.PvM);

                client.Character.Map.Engine.AddFightOnMap(fight);
                client.Character.Map.Engine.RemovePlayer(client);
                fight.EnableContext();
                client.Send("GA;901;" + client.Character.ID + ";" + group.ID);
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant start monster battle : " + e.ToString());
                client.Character.Fighter = null;
            }
        }

        public static void RequestJoinChallenge(WorldClient client, string packet)
        {
            string[] data = packet.Split(';');
            int fightID = int.Parse(data[0]);
            Game.Fights.Fight fight = client.Character.Map.Engine.GetFight(fightID);
            if (client.Character.Fighter != null)
            {
                return;
            }
            if (data.Length > 1)
            {
                int teamID = int.Parse(data[1]);
                if (fight != null)
                {
                    if (!client.Action.IsOccuped)
                    {
                        if (fight.State == Game.Fights.Fight.FightState.PlacementsPhase)
                        {
                            if (fight.FightType == Enums.FightTypeEnum.Agression && client.Character.FactionID == 0)
                            {
                                client.Action.SystemMessage("Impossible de rejoindre le combat, car vous etes neutre !");
                                return;
                            }
                            var teamRequest = fight.GetTeam(teamID);
                            var fighter = new Game.Fights.Fighter(client);
                            if (teamRequest.Restrictions.CanJoin(fighter))
                            {
                                client.Character.Map.Engine.RemovePlayer(client);
                                fight.AddPlayer(fighter, teamID);
                            }
                            else
                            {
                                client.Action.SystemMessage("Impossible de rejoindre le combat, celui ci est bloquer !");
                                client.Character.Fighter = null;
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de rejoindre le combat");
                        }
                    }
                    else
                    {
                        client.Send("BN");
                    }
                }
            }
            else
            {
                if (fight != null)
                {
                    if (fight.State == Game.Fights.Fight.FightState.Fighting)
                    {
                        if (!fight.BlockSpectator)
                        {
                            if (!client.Action.IsOccuped)
                            {
                                client.Character.Map.Engine.RemovePlayer(client);
                                client.Action.Spectator = new Game.Fights.FightSpectator(client, fight);
                                fight.AddSpectator(client.Action.Spectator);
                            }
                            else
                            {
                                client.Send("BN");
                            }
                        }
                        else
                        {
                            client.Send("GA;903;f");
                        }
                    }
                    else
                    {
                        client.Action.SystemMessage("Le combat n'as pas encore debuter, veuilliez attendre !");
                    }
                }
            }
        }

        public static void RequestCastSpell(WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                string[] data = packet.Split(';');

                int spellID = int.Parse(data[0]);
                int cellID = int.Parse(data[1]);

                if (client.Character.Spells.HaveSpell(spellID))
                {
                    if (client.Character.Fighter.Team.Fight.TimeLine.CurrentFighter.Character.ID ==
                        client.Character.ID)
                    {
                        //Checking turn OK and spellbook OK, the player was ready to cast the spell
                        Game.Spells.WorldSpell spellData = client.Character.Spells.GetSpell(spellID);

                        //Try to cast spell in fight engine
                        client.Character.Fighter.Team.Fight.CastSpell
                            (client.Character.Fighter, spellData, spellData.Level, cellID); 
                    }
                }
            }
            else
            {
                client.Close();
            }
        }

        public static void RequestUseWeapon(WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                int cellID = int.Parse(packet);
                if (client.Character.Fighter.Team.Fight.TimeLine.CurrentFighter.Character.ID ==
                        client.Character.ID)
                {
                    client.Character.Fighter.Team.Fight.UseWeapon(client.Character.Fighter, cellID);
                }
            }
        }

        public static void RequestChangeFactionState(WorldClient client, string packet)
        {
            string state = packet.Substring(2);
            switch (state)
            {
                case "+":
                    client.Character.Faction.SetEnabled(true);
                    break;

                case "*":
                    client.Character.Faction.SetEnabled(false);
                    break;
            }
        }

        public static void RequestFightsList(WorldClient client, string packet)
        {
            if (client.Character != null && client.Character.Map != null)
            {
                client.Character.Map.Engine.ShowFightListInfos(client);
            }
        }

        public static void RequestFightsDetails(WorldClient client, string packet)
        {
            if (client.Character != null && client.Character.Map != null)
            {
                int fightID = int.Parse(packet.Substring(2));
                Game.Fights.Fight fightWatched = client.Character.Map.Engine.GetFight(fightID);
                if (fightWatched != null)
                {
                    client.Send("fD" + fightWatched.DisplayFightDetails);
                }
                else
                {
                    client.Action.SystemMessage("Combat non disponible !");
                }
            }
        }

        public static void RequestAggresion(WorldClient client, string packet)
        {
            if(Game.Pvp.PvpManager.IsNoPvpMap(client.Character.MapID))
            {
                client.Action.SystemMessage("Vous ne pouvez pas pvp sur cette carte !");
                return;
            }
            if (client.Character.Fighter != null)
            {
                return;
            }
            Network.WorldClient otherClient = client.Character.Map.Engine.GetClientOnMap(int.Parse(packet));
            if (otherClient != null)
            {
                if (Utilities.ConfigurationManager.GetBoolValue("EnableMultiIPSecurity"))
                {
                    if (client.IP == otherClient.IP)
                    {
                        client.Action.SystemMessage("Impossible vous défier vous même !");
                        return;
                    }
                }
                if (otherClient.Character.Fighter != null)
                {
                    return;
                }
                if (!otherClient.Action.IsOccuped)
                {
                    if (!ConfigurationManager.GetBoolValue("EnableNeutreAggro") && otherClient.Character.Faction.ID == Enums.FactionTypeEnum.Neutral)
                    {
                        client.Action.SystemMessage("Impossible d'agresser ce joueur car il est neutre et le serveur n'autorise pas ce type d'agression !");
                        return;
                    }
                    if (otherClient.Character.Faction.ID != client.Character.Faction.ID)
                    {
                        client.Send("GA;901;" + client.Character.ID + ";" + otherClient.Character.ID);
                        otherClient.Send("GA;901;" + otherClient.Character.ID + ";" + client.Character.ID);

                        /* Starting battle engine */
                        Game.Fights.Fight fight = new Game.Fights.Fight(new Game.Fights.Fighter(client),
                                                                        new Game.Fights.Fighter(otherClient),
                                                                        client.Character.Map.Engine,
                                                                        Enums.FightTypeEnum.Agression);

                        client.Character.Map.Engine.AddFightOnMap(fight);

                        /* Destroy on the map */
                        client.Character.Map.Engine.RemovePlayer(client);
                        otherClient.Character.Map.Engine.RemovePlayer(otherClient);

                        /* Enable fight context */
                        fight.EnableContext();
                    }
                    else
                    {
                        client.Action.SystemMessage("Impossible d'agresser ce joueur car celui ci est du meme alignement que vous !");
                    }
                }
            }
            else
            {
                client.Action.SystemMessage("Impossible d'agresser ce joueur car celui ci est indisponible ou introuvable !");
            }
        }

        public static void RequestChangeOrientation(WorldClient client, string packet)
        {
            client.Action.RefreshDirection(int.Parse(packet.Substring(2)));
        }
    }
}
