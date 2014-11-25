using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("monster_templates")]
    public class MonstersTemplateRecord : ActiveRecordBase<MonstersTemplateRecord>
    {
        [PrimaryKey("id")]
        public int ID
        {
            get;
            set;
        }

        [Property("color1")]
        public int Color1
        {
            get;
            set;
        }

        [Property("color2")]
        public int Color2
        {
            get;
            set;
        }

        [Property("color3")]
        public int Color3
        {
            get;
            set;
        }

        [Property("skin")]
        public int Skin
        {
            get;
            set;
        }

        [Property("nom")]
        public string Name
        {
            get;
            set;
        }

        [Property("exp")]
        public long Exp
        {
            get;
            set;
        }

        [Property("kamas")]
        public string Kamas
        {
            get;
            set;
        }

        [Property("drop")]
        public string Drops
        {
            get;
            set;
        }

        [Property("ai")]
        public int AI
        {
            get;
            set;
        }

        [Property("script")]
        public string Script
        {
            get;
            set;
        }

        #region NoDb

        public List<MonsterLevelRecord> Levels = new List<MonsterLevelRecord>();
        public List<DropRecord> MonsterDrops = new List<DropRecord>();

        public MonsterLevelRecord GetLevel(int level)
        {
            if (this.Levels.FindAll(x => x.Level == level).Count > 0)
                return this.Levels.FirstOrDefault(x => x.Level == level);
            return null;
        }

        public MonsterLevelRecord GetRandomLevel()
        {
            if (this.Levels.Count > 0)
            {
                int randomLevel = Utilities.Basic.Rand(0, this.Levels.Count - 1);
                return this.Levels[randomLevel];
            }
            else
            {
                return null;
            }
        }

        public int[] IntervallKamas
        {
            get
            {
                int[] kamas = new int[2];
                string[] data = Kamas.Split(',');
                kamas[0] = int.Parse(data[0]);
                kamas[1] = int.Parse(data[1]);
                return kamas;
            }
        }

        public bool HasScriptAI()
        {
            return Script != "";
        }

        #endregion

    }
}
