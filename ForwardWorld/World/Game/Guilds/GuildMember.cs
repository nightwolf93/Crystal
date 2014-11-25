using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Guilds
{
    public class GuildMember
    {

        #region Fields

        public int ID { get; set; }
        public Network.WorldClient Client { get; set; }

        public string CharacterName { get; set; } // Need for search player when is offline
        public Database.Records.CharacterRecord Character { get; set; }

        public int NetworkState = 0; // 0 = Offline, 1 = Online

        public Guild OwnGuild { get; set; }
        public GuildRank Rank { get; set; }

        public List<int> Rights = new List<int>();

        #endregion

        #region Builders

        public GuildMember(Network.WorldClient client, Guild guild)
        {
            this.OwnGuild = guild;
            this.SetOnline(client);
        }

        public GuildMember(Database.Records.CharacterRecord character, Guild guild)
        {
            this.OwnGuild = guild;
            this.Character = character;
            this.ID = character.ID;
            this.Rank = (GuildRank)character.GuildRank;
            character.GuildRights.Split(',').ToList().FindAll(x => x != "").ForEach(x => this.Rights.Add(int.Parse(x)));
            this.CharacterName = character.Nickname;
            this.SetOffline();
        }

        #endregion

        #region Methods

        public void Send(string packet)
        {
            if (this.Client != null)
            {
                this.Client.Send(packet);
            }
        }

        public void SetOnline(Network.WorldClient client)
        {
            this.Client = client;
            this.ID = client.Character.ID;
            this.Character = client.Character;
            this.CharacterName = client.Character.Nickname;
            this.NetworkState = 1;
            this.RefreshCharacterInfos();
        }

        public void SetOffline()
        {
            this.Client = null;
            this.NetworkState = 0;
        }

        public void Save()
        {
            if (this.Character != null)
            {
                this.Character.GuildID = this.OwnGuild.ID;
                this.Character.GuildRank = (int)this.Rank;
                this.Character.GuildRights = string.Join(",", this.Rights);
                this.Character.SaveAndFlush();
            }
        }

        public void RefreshCharacterInfos()
        {
            Character.GuildRank = (int)Rank;
        }

        public int GetIntRights
        {
            get
            {
                int iRights = 0;
                this.Rights.ForEach(x => iRights += x);
                return iRights;
            }
        }

        public void AllowFullRight()
        {
            this.Rights.Clear();
            this.Rights.Add(GuildRightsConstants.CAN_BOOST);
            this.Rights.Add(GuildRightsConstants.CAN_COLLECT_COLLECTOR);
            this.Rights.Add(GuildRightsConstants.CAN_INVITE);
            this.Rights.Add(GuildRightsConstants.CAN_KICK);
            this.Rights.Add(GuildRightsConstants.CAN_MODIFY_MOUNTPARK);
            this.Rights.Add(GuildRightsConstants.CAN_SET_MY_XP);
            this.Rights.Add(GuildRightsConstants.CAN_SET_RANK);
            this.Rights.Add(GuildRightsConstants.CAN_SET_RIGHTS);
            this.Rights.Add(GuildRightsConstants.CAN_SET_XP);
            this.Rights.Add(GuildRightsConstants.CAN_USE_COLLECTOR);
            this.Rights.Add(GuildRightsConstants.CAN_USE_MOUNTPARK);
            this.Rights.Add(GuildRightsConstants.CAN_USE_MOUNTS);
        }

        public bool HaveRight(int right)
        {
            return this.Rights.Contains(right);
        }

        public List<int> GetRightsByInt(int iRights)
        {
            List<int> rights = new List<int>();
            int timeOut = 0;
            while (iRights > 0)
            {
                foreach (int baseRight in GuildRightsConstants.FullRights)
                {
                    if (iRights >= baseRight)
                    {
                        rights.Add(baseRight);
                        iRights -= baseRight;
                    }
                }
                timeOut++;
                if (timeOut > 100)
                    break;
            }
            return rights;
        }

        #endregion

        #region Patterns

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(this.ID)
                .Append(";")
                .Append(this.CharacterName)
                .Append(";")
                .Append(this.Character.Level)
                .Append(";")
                .Append(this.Character.Look)
                .Append(";")
                .Append((int)this.Rank)
                .Append(";")
                .Append(0)//Xp to give
                .Append(";")
                .Append(0)//Xp gived
                .Append(";")
                .Append(this.GetIntRights)//Rights
                .Append(";")
                .Append(this.NetworkState)
                .Append(";")
                .Append(this.Character.FactionID)
                .Append(";")
                .Append("0;");
            return str.ToString();
        }

        #endregion

    }
}
