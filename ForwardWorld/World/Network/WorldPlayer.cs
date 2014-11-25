using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.World.Game.Jobs;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public class WorldPlayer
    {

        #region Fields

        private WorldClient _client;

        public bool Away = false;

        public int ExchangeNpcID = -1;
        public int InvitedPartyPlayer = -1;
        public int RequestChallengerID = -1;
        public int InvitedGuildPlayer = -1;
        public string LastMessage = "";
        public long LastTradeMessage = 0;
        public long LastGlobalMessage = 0;
        public long LastRecruitementMessage = 0;
        public int RegenBaseTime { get; set; }
        public List<string> RegisteredChannels = new List<string>();

        public Game.Exchange.PlayerExchange CurrentExchange { get; set; }

        public Database.Records.AuctionHouseRecord CurrentAuctionHouse { get; set; }
        public Game.AuctionHouses.AuctionHousePriceArray CurrentAuctionItem { get; set; }

        public Game.Items.ItemBag CurrentExploredBag { get; set; }
        public int KamasExchangeStack = 0;
        public bool ValidateExchange = false;

        public bool Mounted = false;

        public Game.Guilds.Guild Guild { get; set; }
        public Game.Guilds.GuildMember GuildMember { get; set; }

        public Game.Fights.FightSpectator Spectator { get; set; }

        public bool SubscribedToKolizeum = false;
        public int KolizeumLastMapID { get; set; }
        public int KolizeumLastCellID { get; set; }

        public JobSkill NextJobSkill { get; set; }
        public JobCraftSkill CurrentJobCraftSkill { get; set; }

        public bool GodMode = false;

        #endregion

        #region Builders

        public WorldPlayer(WorldClient client)
        {
            _client = client;
            RegisteredChannels.Add(Enums.ChannelEnum.Default);
            RegisteredChannels.Add(Enums.ChannelEnum.Guild);
            RegisteredChannels.Add(Enums.ChannelEnum.Trade);
            RegisteredChannels.Add(Enums.ChannelEnum.Party);
            RegisteredChannels.Add(Enums.ChannelEnum.Recruitment);
            RegisteredChannels.Add(Enums.ChannelEnum.Admin);
        }

        #endregion

        #region Message

        public void SystemMessage(string message)
        {
            _client.Send("cs<font color=\"" + Utilities.ConfigurationManager.GetStringValue("SystemMessageColor") +"\">" + message + "</font>");
        }

        public void BasicMessage(string message, string color)
        {
            _client.Send("cs<font color=\"" + color + "\">" + message + "</font>");
        }

        public void KolizeumMessage(string message)
        {
            _client.Send("cs<font color=\"#FF6633\"><b>[" + Utilities.ConfigurationManager.GetStringValue("KolizeumName") + "]</b> : " + message + "</font>");
        }

        public void GodMessage(string message)
        {
            if(this.GodMode)
                this.BasicMessage("<b>[GOD]</b> " + message, "#044A40");
        }

        public void NotifMessage(string notif)
        {
            //102Text|Color
            _client.Send("102" + notif + "|000000");
        }

        #endregion

        #region Character

        public void Regen(int count, bool full = false)
        {
            if (full)
            {
                int restoredPoints = _client.Character.Stats.MaxLife - _client.Character.CurrentLife;
                _client.Character.CurrentLife = _client.Character.Stats.MaxLife;
                _client.SendImPacket("01", restoredPoints.ToString());
            }
            else
            {
                if (_client.Character.CurrentLife + count > _client.Character.Stats.MaxLife)
                {
                    count = _client.Character.Stats.MaxLife - _client.Character.CurrentLife;
                    _client.Character.CurrentLife = _client.Character.Stats.MaxLife;
                }
                else
                {
                    _client.Character.CurrentLife += count;
                }
                _client.SendImPacket("01", count.ToString());
            }
            _client.Character.Stats.RefreshStats();
        }

        public void RemoveKamas(int amount)
        {
            _client.Character.Kamas -= amount;
            _client.SendImPacket("046", amount.ToString());
            _client.Character.Stats.RefreshStats();
        }

        public void AddKamas(int amount)
        {
            _client.Character.Kamas += amount;
            _client.SendImPacket("045", amount.ToString());
            _client.Character.Stats.RefreshStats();
        }

        public void AddExp(long exp)
        {
            if (Helper.ExpFloorHelper.GetNextCharactersLevelFloor(_client.Character.Level) == null)
                return;
            _client.Character.Experience += exp;
            TryLevelUp();
        }

        public void SaveContents()
        {
            foreach (Database.Records.WorldItemRecord item in _client.Character.Items.Items)
            {
                try
                {
                    item.SaveAndFlush();
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Error with the item : " + e.ToString());
                }           
            }
            try
            {
                _client.AccountData.SaveAndFlush();
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't save account data !");
            }
        }

        public void SaveCharacter()
        {
            if (_client.Character != null)
            {
                _client.Character.UpdateAndFlush();
            }
        }

        public bool IsOverPod
        {
            get
            {
                return GetCurrentPods > GetMaxPods;
            }
        }

        public int GetMaxPods
        {
            get          
            {
                return 1000 + Math.Abs((_client.Character.Stats.Strenght.Total / 10)) + this._client.Character.Stats.PodsBonus.Total;
            }
        }

        public int GetCurrentPods
        {
            get
            {
                int totalPods = 0;
                _client.Character.Items.Items.ForEach(x => totalPods += (x.GetTemplate.Weight * x.Quantity));
                return totalPods;
            }
        }

        public void TryLevelUp()
        {
            if (Helper.ExpFloorHelper.GetCharactersFloor(this._client.Character.Experience).ID == this._client.Character.Level) return;

            Database.Records.ExpFloorRecord floor = Helper.ExpFloorHelper.GetCharactersFloor(this._client.Character.Experience);
            this._client.Character.CaractPoint += (floor.ID - this._client.Character.Level) * 5;
            this._client.Character.SpellPoint += (floor.ID - this._client.Character.Level);
            this._client.Character.Level = floor.ID;
            this._client.Send("AN" + this._client.Character.Level);
            this.Regen(0, true);//Patched
            this._client.Character.Spells.LearnBaseSpell();
            this._client.Character.Stats.RefreshStats();
        }

        public void Teleport(int mapid, int cellid)
        {
            World.GoToMap(this._client, mapid, cellid);
        }

        public void StartAutoRegen(int intervall = 1000)
        {
            EndAutoRegen();
            RegenBaseTime = Environment.TickCount;
            this._client.Send("ILS" + intervall);
        }

        public void EndAutoRegen()
        {
            if (RegenBaseTime != 0)
            {
                try
                {
                    var regenTime = (Environment.TickCount - RegenBaseTime) / 1000;
                    this.Regen((int)regenTime);
                    RegenBaseTime = 0;
                    this._client.Send("ILS" + int.MaxValue);
                    this._client.Character.Stats.RefreshStats();
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't apply regen : " + e.ToString());
                }
            }
        }

        public void OpenBank()
        {
            Game.Exchange.Bank.OpenBank(this._client, this._client.AccountData.Bank);
        }

        public void ClearBagExplored()
        {
            this.CurrentExploredBag = null;
        }

        #endregion

        #region Mount

        public void SendMountPanel()
        {
            if (_client.Character.Mount != null)
            {
                _client.Send(_client.Character.Pattern.ShowMountPanel);
            }
            else
            {
                _client.Send("Re-");
            }
        }

        #endregion

        #region Refresh

        public void RefreshPods()
        {
            _client.Send("Ow" + GetCurrentPods + "|" + GetMaxPods);
        }

        public void RefreshCharacter()
        {
            if (_client.Character.Map != null)
            {
                _client.Character.Map.Engine.Send("Oa" + _client.Character.ID + "|" + _client.Character.Items.DisplayItem());
            }
        }

        public void RefreshRoleplayEntity(bool smoke = true)
        {
            if (_client.Character.Map != null)
            {
                var s = "+";
                if (smoke)
                    s = "~";

                _client.Character.Map.Engine.Send("GM|" + s + _client.Character.Pattern.ShowCharacterOnMap);
            }
        }

        public void RefreshDirection(int dir)
        {
            if (_client.Character.Map != null)
            {
                _client.Character.Direction = dir;
                _client.Character.Map.Engine.Send("eD" + _client.Character.ID + "|" + dir);
            }
        }

        public void RefreshCharacterJob(bool tool = false)
        {
            if (_client.Character != null)
            {
                _client.Send("OT");
                foreach (var job in _client.Character.Jobs)
                {
                    if (tool)
                    {
                        job.SendJobTool(_client);
                    }
                    else
                    {
                        job.SendJob(_client);
                    }
                }
            }
        }

        #endregion

        #region State

        public bool IsOccuped
        {
            get
            {
                if (_client.State == WorldClientState.OnDialog || _client.State == WorldClientState.OnMove
                    || _client.State == WorldClientState.OnRequestZaap || _client.State == WorldClientState.OnExchangePnj
                    || InvitedPartyPlayer != -1 || RequestChallengerID != -1 || this.CurrentExploredBag != null || this.CurrentAuctionHouse != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Hotel

        public void CheckHotelRoom()
        {
            if (_client.Character != null)
            {
                var r = Game.Hotel.HotelManager.GetRoomByOwner(_client.Character.Nickname);
                if (r != null)
                {
                    r.UnLocate(null, null);
                }
            }
        }

        #endregion

    }
}
