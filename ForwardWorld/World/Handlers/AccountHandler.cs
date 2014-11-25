using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Zivsoft.Log;
using NHibernate.Criterion;
using NHibernate.Engine;
using Crystal.WorldServer.Utilities;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class AccountHandler
    {
        public static List<string> AllowedChar = new List<string>() { "a", "z", "e", "r", "t", "y", "u", "i", "o", "p", "q", "s",
        "d", "f", "g", "h", "j", "k", "l", "m", "w", "x", "c", "v", "b", "n", "-"};

        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("AT", typeof(AccountHandler).GetMethod("CheckTicket"));
            Network.Dispatcher.RegisteredMethods.Add("AP", typeof(AccountHandler).GetMethod("SendCharacterRandomName"));
            Network.Dispatcher.RegisteredMethods.Add("AA", typeof(AccountHandler).GetMethod("CreateCharacter"));
            Network.Dispatcher.RegisteredMethods.Add("AD", typeof(AccountHandler).GetMethod("DeleteCharacter"));
            Network.Dispatcher.RegisteredMethods.Add("ALf", typeof(AccountHandler).GetMethod("SendCharacterList"));
            Network.Dispatcher.RegisteredMethods.Add("AS", typeof(AccountHandler).GetMethod("SelectCharacter"));
            Network.Dispatcher.RegisteredMethods.Add("AB", typeof(AccountHandler).GetMethod("BoostStats"));
        }

        public static void CheckTicket(Network.WorldClient client, string packet)
        {
            string ticket = packet.Substring(2);
            if (Communication.Realm.Communicator.Tickets.ContainsKey(ticket))
            {
                var account = Communication.Realm.Communicator.Tickets[ticket];
                //var connected = World.Helper.WorldHelper.GetClientByAccount(account.Username);
                //if (connected != null)
                //{
                //    connected.Close();
                //}
                client.Account = account;
                Communication.Realm.Communicator.Tickets.Remove(ticket);

                client.Account.Characters = Helper.AccountHelper.GetCharactersForOwner(client.Account.ID);
                client.State = Network.WorldClientState.SelectCharacter;

                if (Helper.AccountHelper.ExistAccountData(client.Account.ID))
                {
                    client.AccountData = Helper.AccountHelper.GetAccountData(client.Account.ID);
                    client.AccountData.Load();
                }
                else
                {
                    client.AccountData = Helper.AccountHelper.CreateNewAccountData(client.Account);
                }

                Communication.Realm.Communicator.Server.MainRealm.SendMessage
                    (new Communication.Realm.Packet.PlayerConnectedMessage(client.Account.Username));

                SendCharacterList(client, null);

                if (Manager.WorldManager.IsBanned(client.Account))
                {
                    client.Close();
                }
            }
            else
            {
                Utilities.ConsoleStyle.Error("Can't found the ticket !");
            }
        }

        public static void SendCharacterList(Network.WorldClient client, string p)
        {
            StringBuilder packet = new StringBuilder("ALK31600000000|" + client.Account.Characters.Count);
            foreach (Database.Records.CharacterRecord character in client.Account.Characters)
            {
                packet.Append("|").Append(character.ID).Append(";").Append(character.Nickname).Append(";").Append(character.Level)
                    .Append(";").Append(character.Look).Append(";").Append(character.Color1).Append(";").Append(character.Color2)
                    .Append(";").Append(character.Color3).Append(";").Append(character.Items.DisplayItem()).Append(";0;1;0;0;");
            }
            client.Send(packet.ToString());
        }

        public static void SendCharacterRandomName(Network.WorldClient client, string packet)
        {
            client.Send("APK" + Helper.AccountHelper.GetRandomName());
        }

        public static void DeleteCharacter(Network.WorldClient client, string packet)
        {
            if (client.Character == null)
            {
                string[] data = packet.Substring(2).Split('|');
                int id = int.Parse(data[0]);

                Database.Records.CharacterRecord character = Helper.AccountHelper.GetCharacter(id);
                character.Items.Items.ForEach(x => x.DeleteAndFlush());
                Database.Cache.CharacterCache.Cache.Remove(character);
                client.Account.Characters.Remove(character);
                character.Delete();

                Communication.Realm.Communicator.Server.MainRealm.SendMessage
                        (new Communication.Realm.Packet.PlayerDeletedCharacterMessage(character.Nickname));

                SendCharacterList(client, packet);
            }
        }

        public static void CreateCharacter(Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');
            if (!Helper.AccountHelper.ExistName(data[0]))
            {
                if (client.Account.Characters.Count < ConfigurationManager.GetIntValue("MaxCharacterPerServer"))
                {
                    if (data[0].Length > 4 && data[0].Length < 15 && HasValidNickname(data[0]) && int.Parse(data[1]) > 0 && int.Parse(data[1]) < 13 && int.Parse(data[2]) > -1 && int.Parse(data[2]) < 2)
                    {
                        Database.Records.CharacterRecord character = new Database.Records.CharacterRecord()
                        {
                            Nickname = data[0],
                            Breed = int.Parse(data[1]),
                            Gender = int.Parse(data[2]),
                            Color1 = int.Parse(data[3]),
                            Color2 = int.Parse(data[4]),
                            Color3 = int.Parse(data[5]),
                            Scal = ConfigurationManager.GetIntValue("StartScal"),
                            Kamas = ConfigurationManager.GetIntValue("StartKamas"),
                            Direction = ConfigurationManager.GetIntValue("StartDir"),
                            CaractPoint = (ConfigurationManager.GetIntValue("StartLevel") * ConfigurationManager.GetIntValue("RateCapitalPoints")) - 5,
                            SpellPoint = (ConfigurationManager.GetIntValue("StartLevel") * ConfigurationManager.GetIntValue("RateSpellPoints")) - 1,
                            Owner = client.Account.ID,
                        };

                        if (client.Account.Vip == 1)
                        {
                            character.Level = 1500;
                        }
                        else
                        {
                            character.Level = ConfigurationManager.GetIntValue("StartLevel");
                        }
                        character.Experience = Helper.ExpFloorHelper.GetCharactersLevelFloor(character.Level).Character;

                        if (Utilities.ConfigurationManager.GetBoolValue("EnableBreedsOriginalStartMap"))
                        {
                            Database.Records.OriginalBreedStartMapRecord originalMap = Helper.MapHelper.GetOriginalBreedStartMap(character.Breed);
                            character.MapID = originalMap.MapID;
                            character.CellID = originalMap.CellID;
                            character.SaveMap = originalMap.MapID;
                            character.SaveCell = originalMap.CellID;
                        }
                        else
                        {
                            character.MapID = ConfigurationManager.GetIntValue("StartMap");
                            character.CellID = ConfigurationManager.GetIntValue("StartCell");
                            character.SaveMap = ConfigurationManager.GetIntValue("StartMap");
                            character.SaveCell = ConfigurationManager.GetIntValue("StartCell");
                        }
                        character.FirstPlay = true;
                        character.Spells.LearnBaseSpell();
                        character.CurrentLife = character.Stats.MaxLife;
                        character.Look = Helper.AccountHelper.GetDefaultLook(character);

                        /* Add character */
                        character.SaveAndFlush();
                        Database.Cache.CharacterCache.Cache.Add(character);
                        client.Account.Characters.Add(character);

                        Communication.Realm.Communicator.Server.MainRealm.SendMessage
                            (new Communication.Realm.Packet.PlayerCreatedCharacterMessage(character));

                        SendCharacterList(client, packet);
                        Utilities.ConsoleStyle.Infos("'" + client.Account.Username + "' has created the character '" + character.Nickname + "'");
                    }
                    else
                    {
                        client.Send("AAEe");
                    }
                }
            }
            else
            {
                client.Send("AAEa");
            }
        }

        public static void SelectCharacter(Network.WorldClient client, string packet)
        {
            if (client.Character == null)
            {
                int id = int.Parse(packet.Substring(2));
                Database.Records.CharacterRecord character = client.Account.Characters.FirstOrDefault(x => x.ID == id);
                if (character != null)
                {
                    if (character.FirstConnection)
                    {
                        // Database.Cache.WorldItemCache.LoadItemsForCharacter(character);
                        character.FirstConnection = false;
                    }

                    StringBuilder selectPacket = new StringBuilder("ASK|");
                    selectPacket.Append(character.ID).Append("|").Append(character.Nickname).Append("|").Append(character.Level).Append("|")
                        .Append(character.Breed).Append("|").Append(character.Look).Append("|").Append(character.Color1)
                        .Append("|").Append(character.Color2).Append("|").Append(character.Color3).Append("||");
                    character.Items.Items.ForEach(x => selectPacket.Append(x.DisplayItem + ";"));
                    selectPacket.Append("|");

                    client.Character = character;
                    character.SetPlayer(client);
                    client.Send(selectPacket.ToString());
                    client.Send("BAPCrystal");                 
                }
            }
        }

        public static bool HasValidNickname(string nickname)
        {
            foreach (var c in nickname)
            {
                if (!AllowedChar.Contains(c.ToString().ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

        public static void BoostStats(Network.WorldClient client, string packet)
        {
            try
            {
                int type = int.Parse(packet.Substring(2));
                BoostCaract(client, type);

                client.Character.Stats.RefreshStats();
            }
            catch (Exception e)
            {
                client.Action.SystemMessage("Impossible d'augmenter votre caracteristique car une erreur interne s'est produite !");
            }
        }

        public static void BoostCaract(Network.WorldClient client, int type)
        {
            Engines.Stats.SingleStats Stat = client.Character.Stats.GetStats(type);
            if (client.Character.CaractPoint <= 0)
                return;

            Database.Records.BreedRecord breed = Helper.BreedHelper.GetBreed(client.Character.Breed);

            if (breed == null)
                return;

            Engines.Breeds.StatFloor floor = breed.Engine.GetFloorForValue((Enums.StatsTypeEnum)type, Stat.Base);

            if (floor == null)
                return;

            if (client.Character.CaractPoint < floor.Cost)
                return;

            Stat.Base += floor.Value;
            client.Character.CaractPoint -= floor.Cost;

            if ((Enums.StatsTypeEnum)type == Enums.StatsTypeEnum.Life)
                client.Character.CurrentLife += floor.Value;
        }
    }
}
