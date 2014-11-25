using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.AbstractClass;
using Crystal.WorldServer.World.Game.Items;
using Crystal.WorldServer.Engines.Map;
using Crystal.WorldServer.World.Helper;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public class WorldClient : AbstractClient, Interop.Plugins.Interfaces.IPlayer
    {

        #region Fields

        private Dispatcher Handler;

        public Database.Records.AccountRecord Account;
        public Database.Records.AccountDataRecord AccountData;
        public Database.Records.CharacterRecord Character;
        public WorldClientState State = WorldClientState.Authentificate;
        public WorldPlayer Action;
        public long LastActionTime { get; set; }

        #endregion

        public WorldClient(SilverSock.SilverSocket socket)
            : base(socket)
        {
            //Set Handler and Player !!
            Handler = new Dispatcher(this);
            Action = new WorldPlayer(this);
            LastActionTime = Environment.TickCount + Program.MaxIdleTime;

            //Send HelloGame
            Send("HG");
        }

        #region Overrides Methods

        public override void Disconnected()
        {
            try
            {       
                if (Character != null)
                {
                    Action.SaveCharacter();
                    Action.SaveContents();
                    if (Action.RequestChallengerID != -1)
                    {
                        Handlers.GameHandler.EndChallengeRequest(this);
                    }
                    if (Action.InvitedGuildPlayer != -1)
                    {
                        Handlers.GuildHandler.CancelInvitation(this);
                    }
                    if (Character.Fighter != null)
                    {
                        Character.Fighter.Team.Fight.FighterDisconnection(Character.Fighter);
                        Character.Fighter = null;
                    }
                    if (Character.Party != null)
                    {
                        Character.Party.RemoveMember(this);
                    }
                    if (this.Action.GuildMember != null)
                    {
                        this.Action.GuildMember.SetOffline();
                        this.Action.GuildMember.Save();
                    }
                    if (this.Action.Guild != null)
                    {
                        this.Action.Guild.Save();
                    }
                    if (this.Action.CurrentExchange != null)
                    {
                        this.Action.CurrentExchange.Exit();
                    }
                    if (Game.Kolizeum.KolizeumManager.IsRegistered(this))
                    {
                        Game.Kolizeum.KolizeumManager.UnSubscribeToKolizeum(this);
                    }
                    Character.Player = null;

                    try
                    {
                        Character.Map.Engine.RemovePlayer(this);
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Failed to remove from map player : " + e.ToString());
                    }
                }

                try
                {
                    if (Account != null)
                    {
                        Communication.Realm.Communicator.Server.MainRealm.SendMessage
                            (new Communication.Realm.Packet.PlayerDisconnectedMessage(Account.Username));
                    }
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Failed to send disconnection to realm : " + e.ToString());
                }

                Manager.WorldManager.Server.Remove(this);
                Utilities.ConsoleStyle.Infos("Client disconnected !");
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Failed to disconnecte player : " + e.ToString());
            }
        }

        public override void DataArrival(byte[] data)
        {
            try
            {
                string noParsedPacket = System.Text.Encoding.Default.GetString(data);
                LastActionTime = Environment.TickCount + Program.MaxIdleTime;
                foreach (string packet in noParsedPacket.Replace("\x0a", "").Split('\x00'))
                {
                    if (packet == "")
                        continue;

                    if (Program.DebugMode)
                    {
                        Utilities.ConsoleStyle.Debug("Received << " + packet);
                    }

                    Handler.Dispatch(packet);
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error(e.ToString());
            }
        }

        #endregion

        #region Methods

        public void SendImPacket(string header, string param = "")
        {
            Send("Im" + header + ";" + param);
        }

        #endregion

        #region Interface Methods

        public void APIModifyKamas(int value)
        {
            if (value < 0)
            {
                this.Action.RemoveKamas(-value);
            }
            else
            {
                this.Action.AddKamas(value);
            }
        }

        public void APITeleport(int mapid, int cellid)
        {
            this.Action.Teleport(mapid, cellid);
        }

        public void APIMessage(string message)
        {
            this.Action.SystemMessage(message);
        }

        public bool APIHaveItem(int templateID)
        {
            return this.Character.Items.HaveItem(templateID);
        }

        public void APIDeleteItem(int templateID, int quantity)
        {
            this.Character.Items.RemoveItem(this.Character.Items.GetItemByTemplate(templateID), quantity);
        }

        public void APIAddItem(int templateID, int quantity)
        {
            Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(this,  templateID);
            this.Character.AddItem(item, quantity);
        }

        public void APISave()
        {
            this.Character.SaveAndFlush();
        }

        public Database.Records.MapRecords APIGetMap()
        {
            return this.Character.Map;
        }

        public void APISavePosition()
        {
            if (this.Character.SaveMap == this.Character.MapID)
                return;
            this.Character.SaveMap = this.Character.MapID;
            this.Character.SaveCell = this.Character.CellID;
            this.SendImPacket("06");
            this.Character.SaveAndFlush();
        }

        public void APIOpenBank()
        {
            this.Action.OpenBank();
        }

        public ItemBag APIGetBank()
        {
            return this.AccountData.Bank;
        }

        public bool APIHaveKamas(int kamas)
        {
            return this.Character.Kamas >= kamas;
        }

        public void APISetLevel(int level)
        {
            Database.Records.ExpFloorRecord floor = Helper.ExpFloorHelper.GetCharactersLevelFloor(level);
            this.Character.Experience = floor.Character;
            this.Action.TryLevelUp();
        }

        public void APIStartMonsterBattle(int battleCell, string pattern)
        {
            MonsterGroup group = new MonsterGroup()
            {
                CellID = battleCell,
            };
            foreach (var monster in pattern.Split('|'))
            {
                if (monster.Trim() != string.Empty)
                {
                    var data = monster.Trim().Split(',');
                    var m = MonsterHelper.GetMonsterTemplate(int.Parse(data[0])).GetLevel(int.Parse(data[1]));
                    group.AddMonster(m);
                }
            }
            Handlers.GameHandler.StartMonstersBattle(this, group);
        }

        public void APIShowCell(int cell)
        {
            this.Send("Gf" + this.Character.ID + "|" + cell);
        }

        #endregion
    }
}
