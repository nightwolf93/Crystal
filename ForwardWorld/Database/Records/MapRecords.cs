using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("maps")]
    public class MapRecords : ActiveRecordBase<MapRecords>
    {

        public MapRecords()
        {
            this.Engine = new Engines.MapEngine(this);
        }

        #region Database Fields

        [PrimaryKey("id")]
        public int ID
        {
            get;
            set;
        }

        [Property("width")]
        public int Width
        {
            get;
            set;
        }

        [Property("heigth")]
        public int Height
        {
            get;
            set;
        }

        [Property("mapData")]
        public string MapData
        {
            get;
            set;
        }

        [Property("decryptkey")]
        public string DecryptKey
        {
            get;
            set;
        }

        [Property("date")]
        public string CreateTime
        {
            get;
            set;
        }

        [Property("monsters")]
        public string Monsters
        {
            get;
            set;
        }

        [Property("groupmaxsize")]
        public int MaximumMonster
        {
            get;
            set;
        }

        [Property("numgroup")]
        public int MaximumGroup
        {
            get;
            set;
        }

        [Property("places")]
        public string Places
        {
            get;
            set;
        }

        [Property("mappos")]
        public string Position
        {
            get
            {
                return PosX + "," + PosY + ",0";
            }
            set
            {
                string[] data = value.Split(',');
                PosX = int.Parse(data[0]);
                PosY = int.Parse(data[1]);
            }
        }

        [Property("spawnemitters")]
        public string SpawnEmitters
        {
            get;
            set;
        }

        [Property("fixed")]
        public int FixedGroup
        {
            get;
            set;
        }

        #endregion

        #region Fields

        public Engines.MapEngine Engine;

        public List<TriggerRecord> Triggers = new List<TriggerRecord>();

        public int PosX
        {
            get;
            set;
        }

        public int PosY
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public Database.Records.NpcPositionRecord APIFindNpc(int id)
        {
            return this.Engine.GetNpcByTemplate(id);
        }

        public void APISetGroupBonus(int bonus)
        {
            foreach (var group in this.Engine.Spawner.GroupsOnMap)
            {
                group.Bonus = bonus;
                group.CreatePattern();
            }
        }

        public bool APIHavePlayerOnCell(int cell)
        {
            return this.Engine.Players.CharactersOnMap.FindAll(x => x.Character.CellID == cell).Count > 0;
        }

        #endregion

    }
}
