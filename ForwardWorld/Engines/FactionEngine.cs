using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines
{
    public class FactionEngine
    {
        public Database.Records.CharacterRecord Character { get; set; }

        /* Faction Fields */
        public Enums.FactionTypeEnum ID { get { return (Enums.FactionTypeEnum)this.Character.FactionID; } set { this.Character.FactionID = (int)value; } }

        public int Power { get { return this.Character.FactionPower; } set { this.Character.FactionPower = value; } }
        public int Honor { get { return this.Character.FactionHonor; } set { this.Character.FactionHonor = value; } }
        public int Deshonor { get { return this.Character.FactionDeshonor; } set { this.Character.FactionDeshonor = value; } }
        public bool Enabled { get { return this.Character.FactionEnabled; } set { this.Character.FactionEnabled = value; } }

        public Database.Records.ExpFloorRecord Floor { get { return World.Helper.ExpFloorHelper.GetCharactersLevelFloor(this.Power); } }

        public FactionEngine(Database.Records.CharacterRecord character)
        {
            this.Character = character;
        }

        #region Getters

        public bool HaveDeshornor
        {
            get
            {
                return this.Deshonor > 0;
            }
        }

        public string Wings
        {
            get
            {
                return (int)ID + "," + (int)ID + "," + (this.Enabled ? Power.ToString() : "0");
            }
        }

        public string FactionStats
        {
            get
            {
                return (int)ID + "~" + Power + "," + Power + "," + Power + "," + Honor + ",0," + (this.Enabled ? "1" : "0");
            }
        }

        #endregion

        #region Methods

        public void AddExp(int exp)
        {
            this.Honor += exp;
            Character.Player.Send("Im080;" + exp);
            this.Character.Stats.RefreshStats();
            if (World.Helper.ExpFloorHelper.GetCharactersPvPFloor(this.Honor).ID  != this.Floor.ID)
                this.SetRank(World.Helper.ExpFloorHelper.GetCharactersPvPFloor(this.Honor).ID, false, false);
        }

        public void RemoveExp(int exp)
        {
            if (this.Honor - exp < 0)
            {
                this.Honor = 0;
                Character.Player.Send("Im081;" + this.Honor);
            }
            else
            {
                this.Honor -= exp;
                Character.Player.Send("Im081;" + exp);
            }

            if (World.Helper.ExpFloorHelper.GetCharactersPvPFloor(this.Honor).ID != this.Floor.ID)
                this.SetRank(World.Helper.ExpFloorHelper.GetCharactersPvPFloor(this.Honor).ID, true, false);

            this.Character.Stats.RefreshStats();
        }

        public void SetRank(int rank, bool removed = false, bool setHonor = true)
        {
            if (rank < 1)
            {
                rank = 1;
                setHonor = true;
            }

            this.Power = rank;

            if(setHonor)
                this.Honor = this.Floor.Pvp;

            if (removed)
            {
                Character.Player.Send("Im083;" + rank);
            }
            else
            {
                Character.Player.Send("Im082;" + rank);
            }

            this.Character.Stats.RefreshStats();
            this.Character.Player.Action.RefreshRoleplayEntity();
        }

        public void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
            this.Character.Stats.RefreshStats();
            this.Character.Player.Action.RefreshRoleplayEntity();
        }

        public void SetAlign(int align, bool reset = true)
        {
            if (reset)
            {
                this.Power = 1;
                this.Honor = 0;
                this.Deshonor = 0;
                this.Enabled = false;
            }
            this.ID = (Enums.FactionTypeEnum)align;
            this.Character.Stats.RefreshStats();
            this.Character.Player.Action.RefreshRoleplayEntity();
        }

        public bool IsFriendly(World.Network.WorldClient client)
        {
            if (this.ID == Enums.FactionTypeEnum.Neutral)
                return true;

            if (this.ID != client.Character.Faction.ID)
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
