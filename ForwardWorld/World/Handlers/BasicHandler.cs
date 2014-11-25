using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public class BasicHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("BA", typeof(BasicHandler).GetMethod("AdminCommand"));
            Network.Dispatcher.RegisteredMethods.Add("BM", typeof(BasicHandler).GetMethod("ChatMessage"));
            Network.Dispatcher.RegisteredMethods.Add("BaM", typeof(BasicHandler).GetMethod("ExecuteQuickTeleportation"));
            Network.Dispatcher.RegisteredMethods.Add("BYA", typeof(BasicHandler).GetMethod("WantAwayMode"));
            Network.Dispatcher.RegisteredMethods.Add("cC", typeof(BasicHandler).GetMethod("ChangeChannel"));
        }

        public static void AdminCommand(World.Network.WorldClient client, string packet)
        {
            try
            {
                if (client.Account.AdminLevel <= 0) { return; }
                string[] command = packet.Substring(2).Split(' ');
                var rank = Game.Admin.AdminRankManager.GetRank(client.Account.AdminLevel);

                if (rank == null)
                {
                    client.Action.SystemMessage("Votre rang n'est pas definit ! Impossible d'utiliser les commandes !");
                    return;
                }

                if (!rank.HasPermission(command[0].ToLower()))
                {
                    client.Action.SystemMessage("Impossible d'utiliser car le rang de <b>" + rank.Name + "</b> que vous posseder n'as pas les permissions requise !");
                    return;
                }

                if (packet.Substring(2).Contains(")") && packet.Substring(2).Contains("("))
                {
                    Game.ConsoleAPI.ConsoleAPI.Execute(client, packet.Substring(2));
                    return;
                }
                ////temp

                switch (command[0].ToLower())
                {
                    #region Packet

                    case "packet":
                        client.Send(packet.Substring(9));
                        break;

                    #endregion

                    #region Goto

                    case "goto":
                        switch (command.Length)
                        {
                            case 2:
                                Network.WorldClient player = Helper.WorldHelper.GetClientByCharacter(command[1]);
                                if (player != null)
                                {
                                    World.Network.World.GoToMap(client, player.Character.Map, player.Character.CellID);
                                    client.Action.SystemMessage("Teleporter au joueur <b>'" + command[1] + "'</b> !");
                                }
                                else
                                {
                                    client.Action.SystemMessage("Le joueur <b>'" + command[1] + "'</b> n'est pas connecter !");
                                }
                                break;

                            case 3:
                                client.Action.SystemMessage("Teleporter sur la carte ID : <b>" + command[1] + "</b>");
                                Network.World.GoToMap(client, int.Parse(command[1]), int.Parse(command[2]));
                                break;
                        }
                        break;

                    #endregion

                    #region Gome

                    case "gome":
                        var gomePlayer = World.Helper.WorldHelper.GetClientByCharacter(command[1]);
                        if (gomePlayer != null)
                        {
                            if (!gomePlayer.Action.IsOccuped && gomePlayer.Character.Fighter == null)
                            {
                                World.Network.World.GoToMap(gomePlayer, client.Character.MapID, client.Character.CellID);
                            }
                            else
                            {
                                client.Action.SystemMessage("Le joueur est occuper !");
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Le joueur <b>'" + command[1] + "'</b> n'est pas connecter !");
                        }
                        break;

                    #endregion

                    #region Guild

                    case "guild":
                        client.Send("gn");
                        client.Action.SystemMessage("Panel de creation de guilde ouvert !");
                        break;

                    #endregion

                    #region Item

                    case "item":
                        if (client.Account.AdminLevel > 1)
                        {
                            Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, int.Parse(command[1]));
                            client.Character.AddItem(item, int.Parse(command[2]));
                            client.Action.SystemMessage("L'objet <b>" + item.GetTemplate.Name + "</b> a ete correctement ajouter ! ");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de creer l'objet !");
                        }
                        break;

                    #endregion

                    #region ItemSet

                    case "itemset":
                        if (client.Account.AdminLevel > 1)
                        {
                            var set = Game.Sets.ItemManager.GetSet(int.Parse(command[1]));
                            if (set != null)
                            {
                                foreach (var i in set.ItemsList)
                                {
                                    Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, i);
                                    client.Character.AddItem(item, 1);
                                    client.Action.SystemMessage("L'objet <b>" + item.GetTemplate.Name + "</b> a ete correctement ajouter ! ");
                                }
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de creer l'objet !");
                        }
                        break;

                    #endregion

                    #region Look

                    case "look":
                        client.Character.Look = int.Parse(command[1]);
                        client.Action.RefreshRoleplayEntity();
                        break;

                    #endregion

                    #region KillMonsters

                    case "killmonsters":
                        if (client.Character.Fighter != null)
                        {
                            client.Character.Fighter.Team.Fight.BlueTeam.Fighters.ForEach(x => x.EnableDeadState());
                            client.Action.SystemMessage("Pownage des monstres en cours !");
                        }
                        break;

                    #endregion

                    #region Save

                    case "save":
                        World.Network.World.SaveWithThread(null, null);
                        break;

                    #endregion

                    #region Level

                    case "level":
                        Network.WorldClient levelUped;

                        if (command.Length == 3)
                        {
                            levelUped = Helper.WorldHelper.GetClientByCharacter(command[2]);
                        }
                        else
                        {
                            levelUped = client;
                        }
                        if (levelUped != null)
                        {
                            client.Action.SystemMessage("Le niveau a bien ete modifier !");
                            Database.Records.ExpFloorRecord floor = Helper.ExpFloorHelper.GetCharactersLevelFloor(int.Parse(command[1]));
                            levelUped.Character.Experience = floor.Character;
                            levelUped.Action.TryLevelUp();
                        }
                        else
                        {
                            client.Action.SystemMessage("Le joueur n'est pas connecter !");
                        }
                        break;

                    #endregion

                    #region Reload

                    case "reload":
                        switch (command[1])
                        {
                            case "zaaps":
                                Database.Cache.ZaapCache.Init();
                                client.Action.SystemMessage("Les <b>zaaps</b> du monde on ete recharger !");
                                break;

                            case "drooms":
                                Database.Cache.DungeonRoomCache.Init();
                                client.Action.SystemMessage("Les <b>salle de donjons</b> du monde on ete recharger !");
                                break;

                            case "npcs":
                                Database.Cache.NpcCache.Init();
                                Database.Cache.NpcPositionCache.Init();
                                client.Action.SystemMessage("Les <b>Pnjs</b> du monde on ete recharger !");
                                break;

                            case "scripts":
                                Interop.Scripting.ScriptManager.Scripts.Clear();
                                Interop.Scripting.ScriptManager.Load("Scripts");
                                client.Action.SystemMessage("Les <b>Scripts</b> on ete recharger !");
                                break;

                            case "map":
                                int mapid = int.Parse(command[2]);

                                break;

                            case "config":
                                Utilities.ConfigurationManager.LoadConfiguration();
                                client.Action.SystemMessage("La <b>configuration</b> a ete recharger !");
                                break;

                            case "shop":
                                Database.Cache.ShopItemCache.Init();
                                client.Action.SystemMessage("La <b>boutique</b> a ete recharger !");
                                break;

                            case "ads":
                                Game.Ads.AdsManager.LoadAds();
                                client.Action.SystemMessage("Les <b>pubs</b> a ete recharger !");
                                break;

                            case "triggers":
                                Database.Cache.TriggerCache.Init();
                                client.Action.SystemMessage("Les <b>triggers</b> on ete recharger !");
                                break;
                        }
                        break;

                    #endregion

                    #region Learn

                    case "learn":
                        switch (command[1])
                        {
                            case "job":
                                if (command.Length > 2)
                                {
                                    try
                                    {
                                        Game.Jobs.JobManager.LearnJob(client, (Enums.JobsIDEnums)int.Parse(command[2]));
                                    }
                                    catch (Exception e)
                                    {
                                        Utilities.ConsoleStyle.Error("Can't learn job : " + e.ToString());
                                    }
                                }
                                break;

                            case "zaaps":
                                Network.WorldClient player;

                                if (command.Length == 3)
                                {
                                    player = Helper.WorldHelper.GetClientByCharacter(command[2]);
                                }
                                else
                                {
                                    player = client;
                                }

                                if (player != null)
                                {
                                    foreach (Database.Records.ZaapRecord zaap in Database.Cache.ZaapCache.Cache)
                                    {
                                        if (!client.Character.Zaaps.Contains(zaap.MapID))
                                        {
                                            player.Character.Zaaps.Add(zaap.MapID);
                                        }
                                    }
                                    client.Action.SystemMessage("Les zaaps ont ete appris au joueur <b>'" + player.Character.Nickname + "'</b>");
                                    player.Action.SystemMessage("Vous etes desormais en possesion de la connaissance de touts les zaaps !");
                                }
                                else
                                {
                                    client.Action.SystemMessage("Le joueur n'est pas connecter !");
                                }
                                break;

                            case "spell":
                                Network.WorldClient playerNewSpell;
                                if (command.Length == 4)
                                {
                                    playerNewSpell = Helper.WorldHelper.GetClientByCharacter(command[3]);
                                }
                                else
                                {
                                    playerNewSpell = client;
                                }

                                if (playerNewSpell != null)
                                {
                                    Database.Records.SpellRecord spell = World.Helper.SpellHelper.GetSpell(int.Parse(command[2]));
                                    playerNewSpell.Character.Spells.NewSpell(spell.ID, 1, -1);
                                    playerNewSpell.Character.Spells.SendSpells();
                                    client.Action.SystemMessage("Vous avez appris le sort <b>" + spell.Name + "</b>");
                                }
                                else
                                {
                                    client.Action.SystemMessage("Le joueur n'est pas connecter !");
                                }
                                break;
                        }
                        break;

                    #endregion

                    #region Job

                    case "job":
                        switch (command[1])
                        {
                            case "level":
                                var lvlJob = client.Character.Jobs[int.Parse(command[3])];
                                lvlJob.Level = int.Parse(command[2]);
                                lvlJob.Experience = lvlJob.LevelFloor.Job;
                                client.Action.RefreshCharacterJob();
                                client.Action.SystemMessage("Le metier " + ((Enums.JobsIDEnums)lvlJob.JobID).ToString() + " est desormais de niveau " + command[2]);
                                break;
                        }
                        break;

                    #endregion

                    #region Maintenance

                    case "maintenance":
                        {
                            switch (command[1])
                            {
                                case "true":
                                    {
                                        int time = int.Parse(command[2]);
                                        World.Network.World.MaintenanceWorld(time);
                                        break;
                                    }

                                case "false":
                                    {
                                        World.Network.World.UnMaintenanceWorld(client);
                                        break;
                                    }
                            }
                        }
                        break;

                    #endregion

                    #region Add

                    case "add":
                        switch (command[1])
                        {
                            case "trigger":
                                Database.Records.TriggerRecord newTrigger = new Database.Records.TriggerRecord()
                                {
                                    MapID = client.Character.MapID,
                                    CellID = client.Character.CellID,
                                    NewMap = command[2]
                                };
                                newTrigger.SaveAndFlush();
                                Helper.MapHelper.AssignTrigger(newTrigger);
                                client.Action.SystemMessage("Le <b>trigger</b> a correctement ete ajouter !");
                                break;

                            case "capital":
                                Network.WorldClient playerCapital;
                                if (command.Length == 4)
                                {
                                    playerCapital = Helper.WorldHelper.GetClientByCharacter(command[3]);
                                }
                                else
                                {
                                    playerCapital = client;
                                }

                                if (playerCapital != null)
                                {
                                    int earnedCapital = int.Parse(command[2]);
                                    playerCapital.Character.CaractPoint += earnedCapital;
                                    playerCapital.Action.SystemMessage("Vous avez obtenus <b>" + earnedCapital + "</b> points de capital !");
                                    client.Action.SystemMessage("Points de capital ajouter correctement !");
                                    playerCapital.Character.Stats.RefreshStats();
                                }
                                else
                                {
                                    client.Action.SystemMessage("Le joueur n'est pas connecter !");
                                }
                                break;

                            case "place":
                                client.Character.Map.Engine.SetPlaces();
                                client.Character.Map.Engine.Places[int.Parse(command[2])].Add(client.Character.CellID);
                                client.Character.Map.Engine.SetPlaces();
                                client.Action.SystemMessage("La place de combat a ete correctement ajouter en cell : " + client.Character.CellID);
                                break;
                        }
                        break;

                    #endregion

                    #region God

                    case "god":
                        if (client.Action.GodMode)
                        {
                            client.Action.SystemMessage("<b>Mode dieu desactiver !</b>");
                            client.Action.GodMode = false;
                        }
                        else
                        {
                            client.Action.SystemMessage("<b>Mode dieu activer !</b>");
                            client.Action.GodMode = true;
                        }
                        break;

                    #endregion

                    #region Kamas

                    case "kamas":
                        int amount = int.Parse(command[1]);
                        client.Action.AddKamas(amount);
                        break;

                    #endregion

                    #region Regen

                    case "regen":
                        client.Action.Regen(0, true);
                        break;

                    #endregion

                    #region Find

                    case "find":
                        string criterion = command[2];
                        switch (command[1])
                        {
                            case "item":
                                Database.Cache.ItemCache.Cache.FindAll(x => x.Name.ToLower().Contains(criterion.ToLower()))
                                    .ForEach(x => client.Action.SystemMessage("<b>" + x.Name + "</b> -> " + x.ID));
                                break;
                        }
                        break;

                    #endregion

                    #region Scale

                    case "scale":
                        int size = int.Parse(command[1]);
                        if (size > 0)
                        {
                            client.Character.Scal = size;
                            client.Action.RefreshRoleplayEntity();
                        }
                        else
                        {
                            client.Action.SystemMessage("Taille invalide !");
                        }
                        break;

                    #endregion

                    #region Kick

                    case "kick":
                        Network.WorldClient kickedPlayer = Helper.WorldHelper.GetClientByCharacter(command[1]);

                        //Reason specificated
                        string reason = "";
                        if (command.Length > 2)
                            reason = "<b>Raison : </b>" + command[2];

                        if (kickedPlayer != null)
                        {
                            if (kickedPlayer.Account.AdminLevel < 3)
                            {
                                Manager.WorldManager.SendMessage("Le joueur <b>" + kickedPlayer.Character.Nickname + "</b> a ete kicker du serveur ! " + reason);
                                kickedPlayer.Close();
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Le joueur n'est pas connecter !");
                        }
                        break;

                    #endregion

                    #region Say

                    case "say":
                        List<string> message = command.ToList();
                        message.RemoveAt(0);
                        string strMessage = string.Join(" ", message);
                        Manager.WorldManager.SendMessage("[STAFF] <b>" + client.Character.Nickname + "</b> : " + strMessage, "#FF0000");
                        break;

                    #endregion

                    #region Mute

                    case "mute":
                        Network.WorldClient mutedPlayer = Helper.WorldHelper.GetClientByCharacter(command[1]);
                        if (mutedPlayer != null)
                        {
                            if (!World.Manager.WorldManager.MutedAccount.Contains(mutedPlayer.Account.Username))
                            {
                                Manager.WorldManager.MutedAccount.Add(mutedPlayer.Account.Username);
                                client.Action.SystemMessage("Compte <b>" + mutedPlayer.Account.Username + "</b> muter !");
                            }
                            else
                            {
                                client.Action.SystemMessage("Compte déjà muter !");
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Joueur introuvable !");
                        }
                        break;

                    #endregion

                    #region UnMute

                    case "unmute":
                        Network.WorldClient unmutedPlayer = Helper.WorldHelper.GetClientByCharacter(command[1]);
                        if (unmutedPlayer != null)
                        {
                            if (World.Manager.WorldManager.MutedAccount.Contains(unmutedPlayer.Account.Username))
                            {
                                Manager.WorldManager.MutedAccount.Remove(unmutedPlayer.Account.Username);
                                client.Action.SystemMessage("Compte <b>" + unmutedPlayer.Account.Username + "</b> demuter !");
                            }
                            else
                            {
                                client.Action.SystemMessage("Compte non muter !");
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Joueur introuvable !");
                        }
                        break;

                    #endregion

                    #region Honor

                    case "honor":
                        Network.WorldClient honorplayer;
                        if (command.Length == 3)
                        {
                            honorplayer = Helper.WorldHelper.GetClientByCharacter(command[2]);
                        }
                        else
                        {
                            honorplayer = client;
                        }
                        if (honorplayer != null)
                        {
                            if (client.Character.Faction.ID != Enums.FactionTypeEnum.Neutral)
                            {
                                int honorGived = int.Parse(command[1]);
                                client.Character.Faction.AddExp(honorGived);
                                client.Action.SystemMessage("<b>" + honorGived + "</b> point(s) d'honneur ajouter a <b>" + honorplayer.Character.Nickname + "</b>");
                            }
                            else
                            {
                                client.Action.SystemMessage("Impossible l'alignement de la cible est neutre !");
                            }
                        }
                        break;

                    #endregion

                    #region RemoteScript

                    case "remotescript":
                        string url = command[1];
                        WebClient netClient = new WebClient();
                        netClient.DownloadFile(url, "scripts/" + command[2]);
                        Interop.Scripting.ScriptManager.Scripts.Add(new Interop.Scripting.Script("scripts/" + command[2]));
                        client.Action.SystemMessage("Scripts telecharger et installer avec succes");
                        break;

                    #endregion

                    #region Who

                    case "who":
                        StringBuilder whosMessage = new StringBuilder("<b>Liste des joueurs : </b><br />");
                        foreach (var whos in World.Helper.WorldHelper.GetClientsArray)
                        {
                            try
                            {
                                if (whos.Character != null)
                                {
                                    whosMessage.Append("<b>Compte</b> : " + whos.Account.Username + ", <b>Personnage</b> : " + whos.Character.Nickname + ", <b>IP</b> : " + whos.IP + "<br />");
                                }
                            }
                            catch (Exception e) { }
                        }
                        whosMessage.Append("<br />");
                        client.Action.SystemMessage(whosMessage.ToString());
                        break;

                    #endregion

                    #region Warnpools

                    case "warnpools":
                        client.Action.SystemMessage("Avertissement en cours d'envois ...");
                        Game.Pools.PoolManager.WarnerTimer_Elapsed(null, null);
                        client.Action.SystemMessage("Avertissement envoyer !");
                        break;

                    #endregion

                    #region Ban

                    case "ban":
                        Network.WorldClient bannedPlayer = Helper.WorldHelper.GetClientByCharacter(command[1]);
                        if (bannedPlayer != null)
                        {
                            if (bannedPlayer.Account.AdminLevel == 0)
                            {
                                Manager.WorldManager.SendMessage("Le joueur <b>" + bannedPlayer.Character.Nickname + "</b> a ete exclu du serveur !");
                                var newBAccount = new Database.Records.BannedAccountRecord()
                                {
                                    Account = bannedPlayer.Account.Username,
                                };
                                Database.Cache.BannedAccountCache.Cache.Add(newBAccount);
                                newBAccount.SaveAndFlush();
                                bannedPlayer.Close();
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Le joueur n'est pas connecter !");
                        }
                        break;

                    #endregion

                    #region Title

                    case "title":
                        Network.WorldClient titledClient;

                        if (command.Length == 3)
                        {
                            titledClient = Helper.WorldHelper.GetClientByCharacter(command[2]);
                        }
                        else
                        {
                            titledClient = client;
                        }
                        if (titledClient != null)
                        {
                            client.Action.SystemMessage("Le titre a ete modifier");
                            client.Character.TitleID = int.Parse(command[1]);
                            client.Action.RefreshRoleplayEntity();
                        }
                        else
                        {
                            client.Action.SystemMessage("Le joueur n'est pas connecter !");
                        }
                        break;

                    #endregion

                    #region Ticket

                    case "ticket":
                        foreach (KeyValuePair<string, string> tickets in Game.Tickets.TicketsManager.Tickets)
                        {
                            client.Action.SystemMessage("Par <b>" + tickets.Key + "</b> : " + tickets.Value.Replace("<", "").Replace(">", ""));
                            System.Threading.Thread.Sleep(200);
                        }
                        break;

                    #endregion

                    #region SpawnMonster

                    case "monsters":
                        var monstersgroup = new Engines.Map.MonsterGroup();
                        monstersgroup.CellID = client.Character.CellID;
                        monstersgroup.Dir = client.Character.Direction;
                        monstersgroup.Bonus = int.Parse(command[1]);
                        monstersgroup.ID = client.Character.Map.Engine.GetActorAvailableID;
                        foreach (var m in command[2].Split(','))
                        {
                            if (m != "")
                            {
                                try
                                {
                                    monstersgroup.AddMonster(Helper.MonsterHelper.GetMonsterTemplate(int.Parse(m)).Levels.FirstOrDefault());
                                }
                                catch (Exception e)
                                {
                                    client.Action.SystemMessage("Le monstre <b>" + m + "</b> n'existe pas en bdd !");
                                }
                            }
                        }
                        monstersgroup.CreatePattern();
                        client.Character.Map.Engine.Spawner.GroupsOnMap.Add(monstersgroup);
                        client.Character.Map.Engine.Players.CharactersOnMap.ForEach(x => client.Character.Map.Engine.ShowMonstersGroup(x));
                        break;

                    case "monsters_fix":
                        client.Character.Map.Monsters = command[1];
                        client.Character.Map.Save();
                        client.Action.SystemMessage("La carte a ete sauvegarder !");
                        break;

                    #endregion

                    #region Demorph

                    case "unlook":
                        client.Character.Look = int.Parse(client.Character.Breed.ToString() + client.Character.Gender.ToString());
                        client.Action.RefreshRoleplayEntity();
                        break;

                    #endregion

                    #region Exit

                    case "exit":
                        Environment.Exit(0);
                        break;

                    #endregion

                    #region Bank

                    case "bank":
                        switch (command[1])
                        {
                            case "kamas":
                                var name = command[2];
                                var bankamount = int.Parse(command[3]);
                                var player = World.Helper.WorldHelper.GetClientByCharacter(name);
                                if (player != null)
                                {
                                    player.AccountData.Bank.Kamas += bankamount;
                                    player.AccountData.Bank.Save();
                                    client.Action.SystemMessage("Kamas ajouter a la banque ! (Now : <b>" + player.AccountData.Bank.Kamas + "</b>)");
                                }
                                else
                                {
                                    client.Action.SystemMessage("Joueur introuvable");
                                }
                                break;

                            case "open":
                                client.Action.OpenBank();
                                break;
                        }
                        break;

                    #endregion

                    #region Auctionhouse

                    case "auction":
                        switch (command[1])
                        {
                            case "fill":
                                var auction = Game.AuctionHouses.AuctionHouseManager.GetAuctionHouse(client.Character.MapID, int.Parse(command[2]));
                                if (auction != null)
                                {
                                    foreach (var t in auction.GetItemTypesFromTypeID())
                                    {
                                        foreach (var item in Database.Cache.ItemCache.Cache.FindAll(x => x.Type == t))
                                        {
                                            var gen = Helper.ItemHelper.GenerateItem(item.ID);
                                            var ahi = new Database.Records.AuctionHouseItemRecord()
                                            {
                                                Owner = -1,
                                                AuctionID = auction.ID,
                                                ItemID = item.ID,
                                                Quantity = 1,
                                                SellPrice = item.Price,
                                                StartTime = 0,
                                                Stats = gen.Engine.StringEffect(),
                                            };
                                            Database.Cache.AuctionHouseItemsCache.Cache.Add(ahi);
                                            ahi.SaveAndFlush();
                                        }
                                    }
                                    client.Action.SystemMessage("Hotel des ventes (ID:" + + auction.ID + ") remplis !");
                                }
                                break;
                        }
                        break;

                    #endregion

                    default:
                        client.Action.SystemMessage("La commande <b>'" + command[0] + "'</b> est inexistante !");
                        break;
                }
            }
            catch (Exception e)
            {
                client.Action.SystemMessage("Erreur lors de l'execution de la commande");
            }
        }

        public static void ChatMessage(World.Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');
            string channel = data[0];
            string message = data[1];
            if (message.Length > 300)
                return;
            if (client.Action.LastGlobalMessage > Environment.TickCount)
                return;
            client.Action.LastGlobalMessage = Environment.TickCount + 3000;
            if (World.Manager.WorldManager.MutedAccount.Contains(client.Account.Username))
            {
                client.Action.SystemMessage("Impossible de parler ! Veuilliez contacter un administrateur via le site !");
                return;
            }
            switch (channel)
            {
                case Enums.ChannelEnum.Default:
                    if (message.StartsWith("."))
                    {
                        string[] command = message.Split(' ');
                        PlayerCommandExecute(command[0].Substring(1), command, client);
                    }
                    else if (message.StartsWith("!"))
                    {
                        if (client.Account.AdminLevel > 0)
                        {
                            string[] command = message.Split(' ');
                            AdminCommand(client, "BA" + message.Substring(1));
                        }
                        else
                        {
                            client.Action.SystemMessage("Vous n'avez pas les permissions pour executer les commandes Admin !");
                        }
                    }
                    else
                    {
                        if (client.Action.LastMessage != message)
                        {
                            if (client.Character.Fighter != null)
                            {
                                client.Character.Fighter.Team.Fight.Send("cMK|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                            }
                            else
                            {
                                client.Character.Map.Engine.Send("cMK|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                            }
                            client.Action.LastMessage = message;
                        }
                        else
                        {
                            client.SendImPacket("184");
                        }
                    }
                    break;

                case Enums.ChannelEnum.Trade:
                    if (client.Action.LastMessage != message)
                    {
                        if (client.Character.Level >= Utilities.ConfigurationManager.GetIntValue("MinLevelToSpeakInWorldChannel"))
                        {

                            if (Environment.TickCount > client.Action.LastTradeMessage)
                            {
                                SendChatMessageToAll(World.Helper.WorldHelper.GetClientsArray.ToList(), Enums.ChannelEnum.Trade, 
                                    "cMK" + Enums.ChannelEnum.Trade + "|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                                client.Action.LastMessage = message;
                                client.Action.LastTradeMessage = Environment.TickCount + Utilities.ConfigurationManager.GetIntValue("TradeMessageInterval");
                            }
                            else
                            {
                                client.Action.SystemMessage("Veuilliez attendre encore un peu avant de pouvoir parler !");
                            }

                        }
                        else
                        {
                            client.SendImPacket("0157", Utilities.ConfigurationManager.GetIntValue("MinLevelToSpeakInWorldChannel").ToString());
                        }
                    }
                    else
                    {
                        client.SendImPacket("184");
                    }
                    break;

                case Enums.ChannelEnum.Recruitment:
                    if (client.Action.LastMessage != message)
                    {
                        if (client.Character.Level >= Utilities.ConfigurationManager.GetIntValue("MinLevelToSpeakInWorldChannel"))
                        {

                            if (Environment.TickCount > client.Action.LastRecruitementMessage)
                            {
                                SendChatMessageToAll(World.Helper.WorldHelper.GetClientsArray.ToList(), Enums.ChannelEnum.Recruitment,
                                    "cMK" + Enums.ChannelEnum.Recruitment + "|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                                client.Action.LastMessage = message;
                                client.Action.LastRecruitementMessage = Environment.TickCount + Utilities.ConfigurationManager.GetIntValue("RecruitementMessageInterval");
                            }
                            else
                            {
                                client.Action.SystemMessage("Veuilliez attendre encore un peu avant de pouvoir parler !");
                            }

                        }
                        else
                        {
                            client.SendImPacket("0157", Utilities.ConfigurationManager.GetIntValue("MinLevelToSpeakInWorldChannel").ToString());
                        }
                    }
                    else
                    {
                        client.SendImPacket("184");
                    }
                    break;

                case Enums.ChannelEnum.Admin:
                    if (client.Account.AdminLevel > 0)
                    {
                        Manager.WorldManager.Server.Clients.FindAll(x => x.Account != null).FindAll(x => x.Account.AdminLevel > 0).ForEach(x => x.Send("cMK" + Enums.ChannelEnum.Admin + "|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message));
                    }
                    break;

                case Enums.ChannelEnum.Party:
                    if (client.Character.Party != null)
                    {
                        SendChatMessageToAll(client.Character.Party.Members, Enums.ChannelEnum.Party, "cMK" + Enums.ChannelEnum.Party + "|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                    }
                    break;

                case Enums.ChannelEnum.Guild:
                    if (client.Action.Guild != null)
                    {
                        client.Action.Guild.Send("cMK" + Enums.ChannelEnum.Guild + "|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                    }
                    break;

                case Enums.ChannelEnum.Osef:

                    break;

                default:
                    string nickname = channel;
                    Network.WorldClient player = Helper.WorldHelper.GetClientByCharacter(nickname);
                    if (player != null)
                    {
                        if (!player.Action.Away && !player.AccountData.EnemiesIDs.Contains(client.AccountData.AccountID))
                        {
                            if (client.Account.AdminLevel == 0)
                            {
                                client.Send("cMKT|" + client.Character.ID + "|" + nickname + "|" + message);
                                player.Send("cMKF|" + client.Character.ID + "|" + client.Character.Nickname + "|" + message);
                            }
                            else
                            {
                                client.Send("cMKT|" + client.Character.ID + "|" + nickname + "|" + message);
                                player.Action.BasicMessage("De <b><a href='asfunction:onHref,ShowPlayerPopupMenu," +
                                            client.Character.Nickname + "'>[MJ]" + client.Character.Nickname + "</a></b> : " + message, "#FF0000");
                            }
                        }
                        else
                        {
                            client.SendImPacket("114", player.Character.Nickname);
                        }
                    }
                    else
                    {
                        client.Send("cMEf" + nickname);
                    }
                    break;
            }

            Interop.PythonScripting.ScriptManager.CallEventPlayerSpeak(client, message, channel);
        }

        public static void SendChatMessageToAll(List<World.Network.WorldClient> clients, string channel, string packet)
        {
            foreach (var client in clients)
            {
                if (client.Action.RegisteredChannels.Contains(channel))
                {
                    client.Send(packet);
                }
            }
        }

        public static void ChangeChannel(World.Network.WorldClient client, string packet)
        {     
            packet = packet.Substring(2);
            var enable = packet[0];
            var type = packet[1];
            if (enable == '+')
            {
                if (!client.Action.RegisteredChannels.Contains(type.ToString()))
                {
                    client.Action.RegisteredChannels.Add(type.ToString());
                }
            }
            else
            {
                if (client.Action.RegisteredChannels.Contains(type.ToString()))
                {
                    client.Action.RegisteredChannels.Remove(type.ToString());
                }
            }
            client.Send("cC" + enable + type);
        }

        public static void PlayerCommandExecute(string label, string[] parameters, World.Network.WorldClient client)
        {
            try
            {
                if (client.Character.Fighter != null)
                {
                    client.Action.SystemMessage("Impossible d'utiliser les commandes en combat");
                    return;
                }
                //if (Utilities.ConfigurationManager.ConfigurationElements.ContainsKey("Allow!" + label))
                //{
                //    if (!Utilities.ConfigurationManager.GetBoolValue("Allow!" + label))
                //    {
                //        client.Action.SystemMessage("La commande n'est pas activer sur ce serveur ! !");
                //        return;
                //    }
                //}
                switch (label)
                {
                    case "infos":
                        client.Action.SystemMessage("Core <b>" + Utilities.ConfigurationManager.GetStringValue("EmulatorName") + "</b> v<b>" + Program.CrystalVersion + "</b> Par NightWolf");
                        client.Action.SystemMessage("Joueurs en ligne : <b>" + Manager.WorldManager.Server.Clients.ToArray().ToList().FindAll(x => x.Account != null).Count + "</b>");
                        client.Action.SystemMessage("Uptime : <b>" + Utilities.Basic.GetUptime() + "</b>");
                        break;

                    case "start":
                        World.Network.World.GoToMap(client, client.Character.SaveMap, client.Character.SaveCell);
                        client.Action.NotifMessage("Vous etes desormais sur la carte de depart !");
                        break;

                    case "save":
                        client.Character.SaveAndFlush();
                        client.Action.SystemMessage("Personnage sauvegarder !");
                        break;

                    case "vie":
                        if (client.Character.Fighter == null)
                        {
                            client.Action.Regen(1000, true);
                            client.Action.SystemMessage("Vos points de vie on ete remis au maximum !");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible de remettre sa vie au maximum en combat !");
                        }
                        break;

                    case "shop":
                        if (client.Character.Fighter == null)
                        {
                            Game.Shop.ShopManager.WantBuyObject(client, int.Parse(parameters[1]));
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible d'acheter en combat !");
                        }
                        break;

                    case "points":
                        client.Action.SystemMessage("Votre solde de <i>" + Utilities.ConfigurationManager.GetStringValue("ShopPointName") + "</i> : " + client.Account.Points);
                        break;


                    case "restat":
                        if (client.Character.Fighter == null)
                        {
                            client.Character.Stats.Life.Base = 0;
                            client.Character.Stats.Wisdom.Base = 0;
                            client.Character.Stats.Strenght.Base = 0;
                            client.Character.Stats.Fire.Base = 0;
                            client.Character.Stats.Water.Base = 0;
                            client.Character.Stats.Agility.Base = 0;
                            client.Character.CaractPoint = ((client.Character.Level * 5) - 5) * (client.Character.EliteLevel + 1);
                            client.Action.SystemMessage("Vos caractéristique ont ete remis a 0 !");
                            client.Character.Stats.RefreshStats();
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible en combat !");
                        }
                        break;

                    case "parcho":
                        if (client.Character.Fighter == null)
                        {
                            if (client.Character.Stats.Life.Base + 101 <= 101)
                            {
                                client.Character.Stats.Life.Base = 101;
                            }
                            if (client.Character.Stats.Strenght.Base + 101 <= 101)
                            {
                                client.Character.Stats.Strenght.Base = 101;
                            }
                            if (client.Character.Stats.Wisdom.Base + 101 <= 101)
                            {
                                client.Character.Stats.Wisdom.Base = 101;
                            }
                            if (client.Character.Stats.Fire.Base + 101 <= 101)
                            {
                                client.Character.Stats.Fire.Base = 101;
                            }
                            if (client.Character.Stats.Water.Base + 101 <= 101)
                            {
                                client.Character.Stats.Water.Base = 101;
                            }
                            if (client.Character.Stats.Agility.Base + 101 <= 101)
                            {
                                client.Character.Stats.Agility.Base = 101;
                            }
                            client.Character.Stats.RefreshStats();
                            client.Action.SystemMessage("Votre personnage a ete parchotter, si cela n'est pas le cas, taper .restat et reesayer !");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible en combat !");
                        }
                        break;

                    case "neutre":
                        if (client.Character.Fighter == null)
                        {
                            client.Character.Faction.SetAlign(0);
                            client.Action.SystemMessage("Vous etes desormais : <b>Neutre</b>");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible en combat !");
                        }
                        break;

                    case "bonta":
                        if (client.Character.Fighter == null)
                        {
                            client.Character.Faction.SetAlign(1);
                            client.Action.SystemMessage("Vous etes desormais : <b>Bontarien</b>");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible en combat !");
                        }
                        break;

                    case "brak":
                        if (client.Character.Fighter == null)
                        {
                            client.Character.Faction.SetAlign(2);
                            client.Action.SystemMessage("Vous etes desormais : <b>Brakmarien</b>");
                        }
                        else
                        {
                            client.Action.SystemMessage("Impossible en combat !");
                        }
                        break;

                    case "fm":
                        var weapon = client.Character.Items.GetItemAtPos(1);
                        if (weapon != null)
                        {
                            switch (parameters[1].ToLower())
                            {
                                case "force":
                                    Game.Items.MagicalForge.FmMyWeapon(client, weapon, 1);
                                    break;

                                case "feu":
                                    Game.Items.MagicalForge.FmMyWeapon(client, weapon, 2);
                                    break;

                                case "eau":
                                    Game.Items.MagicalForge.FmMyWeapon(client, weapon, 3);
                                    break;

                                case "air":
                                    Game.Items.MagicalForge.FmMyWeapon(client, weapon, 4);
                                    break;

                                default:
                                    client.Action.SystemMessage("Ce type de forgemagie n'existe pas !");
                                    break;
                            }
                            client.Action.SystemMessage("Deconnecter vous, et reconnecter vous pour voir votre arme forgemager !");
                        }
                        else
                        {
                            client.Action.SystemMessage("Veuilliez vous equiper d'une arme !");
                        }
                        break;

                    case "addc":
                        var requestedPoints = int.Parse(parameters[2]);
                        if (client.Character.CaractPoint < requestedPoints)
                        {
                            client.Action.SystemMessage("Vous n'avez pas assez de points de caracteristiques !");
                            return;
                        }
                        if (requestedPoints <= 0)
                        {
                            client.Action.SystemMessage("La valeur doit etre superieur a 0 !");
                            return;
                        }
                        for (int i = 0; i < requestedPoints; i++)
                        {
                            switch (parameters[1].ToLower())
                            {
                                case "vie":
                                    AccountHandler.BoostCaract(client, 11);
                                    break;

                                case "sagesse":
                                    AccountHandler.BoostCaract(client, 12);
                                    break;

                                case "force":
                                    AccountHandler.BoostCaract(client, 10);
                                    break;

                                case "feu":
                                    AccountHandler.BoostCaract(client, 15);
                                    break;

                                case "eau":
                                    AccountHandler.BoostCaract(client, 13);
                                    break;

                                case "air":
                                    AccountHandler.BoostCaract(client, 14);
                                    break;

                                default:
                                    client.Action.SystemMessage("Ce type de caracteristique n'existe pas !");
                                    return;
                            }
                        }
                        client.Action.SystemMessage("Vos points on ete correctement ajouter !");
                        //client.Character.CaractPoint -= requestedPoints;
                        client.Character.Stats.RefreshStats();
                        break;

                    case "elite":
                        if (client.Character.Level == 5000)
                        {
                            if (!client.Action.IsOccuped && client.Character.Fighter == null)
                            {
                                Game.Elite.EliteManager.UpElite(client);
                            }
                            else
                            {
                                client.Action.SystemMessage("Vous etes occuper, action impossible !");
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Vous devez etre niveau 5000 pour passer en mode elite !");
                        }
                        break;

                    case "ticket":
                        switch (parameters[1].ToLower())
                        {
                            case "close":
                                Game.Tickets.TicketsManager.CloseTicket(client);
                                break;

                            default:
                                Game.Tickets.TicketsManager.WantAddTicket(client, string.Join(" ", parameters).Replace(".ticket" ,""));
                                break;
                        }
                        break;

                    case "koli":
                    case "k":
                        switch (parameters[1].ToLower())
                        {
                            case "on":
                                Game.Kolizeum.KolizeumManager.SubscribeToKolizeum(client);
                                break;

                            case "off":
                                Game.Kolizeum.KolizeumManager.UnSubscribeToKolizeum(client);
                                break;

                            case "infos":
                                client.Action.SystemMessage("Kolizeum system <b>v1.0</b> par <b>NightWolf</b>");
                                break;
                        }
                        break;

                    case "hotel":
                        switch (parameters[1].ToLower())
                        {
                            case "louer":
                                if (client.Character.MapID == 19001 && client.Character.CellID == 277)
                                {
                                    Game.Hotel.HotelManager.LocateRoom(client, parameters[2]);
                                }
                                else
                                {
                                    client.Action.SystemMessage("Veuilliez vous rendre a l'hotel pour louer une salle !");
                                }
                                break;
                                
                            case "mdp":
                                Game.Hotel.HotelManager.ChangeRoomPassword(client, parameters[2]);
                                break;

                            case "join":
                                if (parameters.Length > 3)
                                {
                                    Game.Hotel.HotelManager.GotoRoom(client, parameters[2], parameters[3]);
                                }
                                else
                                {
                                    Game.Hotel.HotelManager.GotoRoom(client, parameters[2]);
                                }
                                break;

                            case "salle":
                                client.Action.SystemMessage("<b>Liste des salles : </b><br />");
                                foreach (var r in Database.Cache.HotelCache.Cache)
                                {
                                    client.Action.SystemMessage("Salle : <b>" + r.Name + "</b>");
                                }
                                break;

                            case "close":
                                Game.Hotel.HotelManager.CloseRoom(client);
                                break;
                        }
                        break;

                    case "help":
                        client.Action.SystemMessage("<b>.start</b> : Retourne au point de depart");
                        client.Action.SystemMessage("<b>.infos</b> : Affiche les informations serveur");
                        client.Action.SystemMessage("<b>.save</b> : Sauvegarde le personnage");
                        client.Action.SystemMessage("<b>.vie</b> : Remet la vie au maximum");
                        //client.Action.SystemMessage("<b>.shop</b> [achatID] : Achete un objet sur la boutique");
                        client.Action.SystemMessage("<b>.points</b> : Affiche les points boutique en possession");
                        client.Action.SystemMessage("<b>.restat</b> : Restat les caracteristiques");
                        client.Action.SystemMessage("<b>.parcho</b> : Parchotte votre personnage a 101 partout");
                        client.Action.SystemMessage("<b>.neutre</b> : Change votre alignement en neutre");
                        client.Action.SystemMessage("<b>.bonta</b> : Change votre alignement en bontarien");
                        client.Action.SystemMessage("<b>.brak</b> : Change votre alignement en brakmarien");
                        client.Action.SystemMessage("<b>.fm (force,feu,air,eau)</b> : Forgemage votre arme");
                        client.Action.SystemMessage("<b>.addc (vie,sagesse,force,feu,air,eau) (points)</b> : Ajoute rapidement des points de caracteristiques");
                        client.Action.SystemMessage("<b>.elite</b> : Permet de monter un niveau elite (Niveau 5000 requis), Vous remet niveau 200 en gardant vos caracteristique");
                        client.Action.SystemMessage("<b>.enclo</b> : Ouvre votre enclo personnel");
                        client.Action.SystemMessage("<b>.ticket (votre message)</b> : Permet d'envoyer un ticket d'aide au moderateurs");
                        client.Action.SystemMessage("<b>.ticket close</b> : Ferme votre ticket et vous permet d'en refaire un nouveau");
                        client.Action.SystemMessage("<b>.k (on | off)</b> : Vous inscrit ou desinscrit du Kolizeum");
                        client.Action.SystemMessage("<b>.goxp</b> : Teleporte a la zone xp");
                        //client.Action.SystemMessage(Program.HelpContent);
                        break;

                    default:
                        try
                        {
                            if (Game.Commands.CommandsManager.ExistCommand(label))
                            {
                                Game.Commands.CommandsManager.CommandsRegistered[label].OnCall(client, parameters);
                            }
                            else
                            {
                                Interop.Scripting.ScriptManager.CallScript("command", label, client, label);
                            }
                        }
                        catch (Exception e)
                        {
                            Utilities.ConsoleStyle.Error(e.ToString());
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                client.Action.SystemMessage("Impossible d'utiliser la commande !");  
            }  
        }

        public static void ExecuteQuickTeleportation(World.Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(3).Split(',');
            int PosX = int.Parse(data[0]);
            int PosY = int.Parse(data[1]);
            Database.Records.MapRecords map = Helper.MapHelper.FindMap(PosX, PosY);
            if (map != null)
            {
                World.Network.World.GoToMap(client, map, client.Character.CellID);
                client.Action.SystemMessage("Teleporter sur la carte : <b>" + PosX + "," + PosY + "</b> (ID : " + map.ID + ")");
            }
            else
            {
                client.Action.SystemMessage("Impossible de trouver la carte en : <b>" + PosX + "," + PosY + "</b>");
            }
        }

        public static void WantAwayMode(World.Network.WorldClient client, string packet)
        {
            if (!client.Action.Away)
            {
                client.Action.Away = true;
                client.SendImPacket("037");
            }
            else
            {
                client.Action.Away = false;
                client.SendImPacket("038");
            }
        }
    }
}
