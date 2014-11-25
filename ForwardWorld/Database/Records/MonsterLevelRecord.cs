using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("monster_levels")]
    public class MonsterLevelRecord : ActiveRecordBase<MonsterLevelRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("templateId")]
        public int TemplateID
        {
            get;
            set;
        }

        [Property("level")]
        public int Level
        {
            get;
            set;
        }

        [Property("pa")]
        public int AP
        {
            get;
            set;
        }

        [Property("pm")]
        public int MP
        {
            get;
            set;
        }

        [Property("vie")]
        public int Life
        {
            get;
            set;
        }

        [Property("size")]
        public int Size
        {
            get;
            set;
        }

        [Property("caracts")]
        public string Stats
        {
            get;
            set;
        }

        [Property("resistances")]
        public string ProtectStats
        {
            get;
            set;
        }

        [Property("spells")]
        public string Spells
        {
            get;
            set;
        }

        #region NoDb

        public bool IsTempLevel = false;
        public MonstersTemplateRecord TempTemplate = null;

        public MonstersTemplateRecord GetTemplate
        {
            get
            {
                if (IsTempLevel)
                {
                    return TempTemplate;
                }
                else
                {
                    return World.Helper.MonsterHelper.GetMonsterTemplate(this.TemplateID);
                }
            }
        }

        public Engines.StatsEngine StatsEngine { get; set; }
        public List<World.Game.Spells.WorldSpell> OwnSpells = new List<World.Game.Spells.WorldSpell>();

        public override string ToString()
        {
            return this.GetTemplate.Name + "(" + this.Level + ")";
        }

        public void InitMonster()
        {
            try
            {
                StatsEngine = new Engines.StatsEngine(this);

                string[] statsData = this.Stats.Split(',');
                StatsEngine.Life.Base = int.Parse(statsData[0]) + this.Life;
                StatsEngine.Strenght.Base = int.Parse(statsData[1]);
                StatsEngine.Fire.Base = int.Parse(statsData[2]);
                StatsEngine.Agility.Base = int.Parse(statsData[3]);
                StatsEngine.Water.Base = int.Parse(statsData[4]);

                string[] data = Spells.Split('|');
                foreach (string s in data)
                {
                    if (s != "")
                    {
                        try
                        {
                            string[] spellData = s.Split(',');
                            int spellID = int.Parse(spellData[0]);
                            int spellLevel = int.Parse(spellData[1]);
                            OwnSpells.Add(new World.Game.Spells.WorldSpell(spellID, spellLevel, 0));
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't load the monster ID : " + this.ID + ", " + e.ToString());
            }
        }

        #endregion
    }
}
