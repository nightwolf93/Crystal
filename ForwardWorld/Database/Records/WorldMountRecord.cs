using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : WorldMountRecord
*/

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("world_mounts")]
    public class WorldMountRecord : ActiveRecordBase<WorldMountRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Owner")]
        public int Owner
        {
            get;
            set;
        }

        [Property("ScrollID")]
        public int ScrollID
        {
            get;
            set;
        }

        [Property("MountType")]
        public int MountType
        {
            get;
            set;
        }

        [Property("Love")]
        public int Love
        {
            get;
            set;
        }

        [Property("Sexe")]
        public int Sexe
        {
            get;
            set;
        }

        [Property("Stamina")]
        public int Stamina
        {
            get;
            set;
        }

        [Property("Level")]
        public int Level
        {
            get;
            set;
        }

        [Property("Exp")]
        public int Exp
        {
            get;
            set;
        }

        [Property("Name")]
        public string Name
        {
            get;
            set;
        }

        [Property("Tired")]
        public int Tired
        {
            get;
            set;
        }

        [Property("Energy")]
        public int Energy
        {
            get;
            set;
        }

        [Property("Reproduction")]
        public int Reproduction
        {
            get;
            set;
        }

        [Property("Maturity")]
        public int Maturity
        {
            get;
            set;
        }

        [Property("Serenity")]
        public int Serenity
        {
            get;
            set;
        }

        [Property("Ancestors")]
        public string Ancestors
        {
            get;
            set;
        }

        public List<Engines.Stats.MountStat> Stats = new List<Engines.Stats.MountStat>();

        public MountTemplateRecord GetTemplate
        {
            get
            {
                return World.Helper.MountHelper.GetMountTemplateByType(this.MountType);
            }
        }

        public string GetMountData
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(this.ID)
                        .Append(":")
                        .Append(MountType)
                        .Append(":")
                        .Append(Ancestors)
                        .Append(":")
                        .Append(",")
                        .Append(":")
                        .Append(Name)
                        .Append(":0:0:" + Level)
                        .Append(":1:1000:0:9500,10000:9501,10000:")
                        .Append(Energy)
                        .Append(",1000:2500,-10000,10000:10000,10000:0:0:" + this.MountEffectToString + ":0,240:0,00:");
                return pattern.ToString();
            }
        }

        public string MountEffectToString
        {
            get
            {           
                return string.Join("," ,this.Stats);//TODO!!
            }
        }

        public void LoadStats()
        {
            this.Stats.Clear();
            foreach (var tEffect in this.GetTemplate.Engine.Stats)
            {
                this.Stats.Add(new Engines.Stats.MountStat() { EffectID = tEffect.EffectID ,
                                            Coef = 1, Value = tEffect.Value * (tEffect.Coef * this.Level)});
            }
        }
    }
}
