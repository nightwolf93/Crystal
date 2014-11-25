using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Guilds
{
    public class Guild
    {

        #region Fields

        public int ID { get; set; }
        public string Name { get; set; }
        public GuildEmblem Emblem { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public string Caracts { get; set; }
        public int CaractsPoints { get; set; }
        public string Spells { get; set; }
        public int SpellsPoints { get; set; }

        public Database.Records.GuildRecord GuildData { get; set; }
        public List<GuildMember> Members = new List<GuildMember>();

        #endregion

        #region Builders

        public Guild(int id, string name, GuildEmblem emblem, Database.Records.GuildRecord guild = null)
        {
            this.ID = id;
            this.Name = name;
            this.Emblem = emblem;
            this.Level = 1;
            this.Experience = 0;
            this.Caracts = ("");
            this.CaractsPoints = 0;
            this.Spells = ("");
            this.SpellsPoints = 0;
            if (guild != null)
            {
                this.GuildData = guild;
            }
            else
            {
                this.GuildData = new Database.Records.GuildRecord();
                this.Save();
                Database.Cache.GuildCache.Cache.Add(this);
            }
        }

        #endregion

        #region Methods

        public void AddMember(Network.WorldClient client)
        {
            //Re-check if client dont have guild
            if (client.Action.Guild == null)
            {
                client.Action.GuildMember = new GuildMember(client, this);
                client.Action.Guild = this;
                client.Character.GuildID = this.GuildData.ID;
                this.SendGuildBasicInformation(client);
                this.Members.Add(client.Action.GuildMember);

                //Si il est pas en combat on refresh l'entite
                if (client.Character.Fighter == null)
                {
                    client.Action.RefreshRoleplayEntity();
                }

                client.Character.Save();
                client.Action.GuildMember.Save();
                this.Save();
            }
        }

        public void KickMember(GuildMember client)
        {
            this.Members.Remove(client);

            client.Character.GuildID = 0;
            client.Character.GuildRank = 0;
            client.Character.GuildRights = "";
            client.Character.SaveAndFlush();
        }

        public void SendGuildBasicInformation(Network.WorldClient client)
        {
            client.Send("gS" + this.Name + "|" + this.Emblem.ToString().Replace(",", "|") + "|" + this.Level);
        }

        public void SendGuildMembersInformations(Network.WorldClient client)
        {
            string membersInfo = string.Join("|", this.Members);
            client.Send("gIM+" + membersInfo);
            this.SendGuildLevelStats(client);
        }

        public void SendGuildLevelStats(Network.WorldClient client)
        {
            client.Send("gIG" + (this.HaveRequiredMembers ? "1" : "0").ToString() + "|" + this.Level + "|" + this.Experience + "|0|0");
        }

        public bool HaveRequiredMembers
        {
            get
            {
                if (Utilities.ConfigurationManager.GetBoolValue("SkipGuildsRestrictions"))
                {
                    return true;
                }
                return this.Members.Count >= 10;
            }
        }

        public void Save()
        {
            try
            {
                this.GuildData.Name = Name;
                this.GuildData.EmblemBackID = Emblem.BackID;
                this.GuildData.EmblemBackColor = Emblem.BackColor;
                this.GuildData.EmblemFrontID = Emblem.FrontID;
                this.GuildData.EmblemFrontColor = Emblem.FrontColor;
                this.GuildData.Level = Level;
                this.GuildData.Exp = Experience;
                this.GuildData.Caracts = Caracts;
                this.GuildData.CaractPoints = CaractsPoints;
                this.GuildData.Spells = Spells;
                this.GuildData.SpellPoints = SpellsPoints;
                this.GuildData.SaveAndFlush();
                this.ID = this.GuildData.ID;
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant't save guild : " + e.ToString());
            }
        }

        public void Delete()
        {

        }

        public void Send(string packet)
        {
            foreach (var member in this.Members)
            {
                member.Send(packet);
            }
        }

        #endregion

        #region Helpers

        public GuildMember FindMember(int id)
        {
            if (this.Members.FindAll(x => x.ID == id).Count > 0)
            {
                return Members.FirstOrDefault(x => x.ID == id);
            }
            return null;
        }

        public GuildMember FindMember(string username)
        {
            if (this.Members.FindAll(x => x.CharacterName == username).Count > 0)
            {
                return Members.FirstOrDefault(x => x.CharacterName == username);
            }
            return null;
        }

        #endregion

        #region Collector


        #endregion

        #region Patterns

        public string DisplayEmblemPattern
        {
            get
            {
                return Utilities.Base36.Encode(Emblem.BackID) + "," + Utilities.Base36.Encode(Emblem.BackColor) + ","
                    + Utilities.Base36.Encode(Emblem.FrontID) + "," + Utilities.Base36.Encode(Emblem.FrontColor);
            }
        }

        #endregion

    }
}
