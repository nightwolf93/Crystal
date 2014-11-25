using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;
using Crystal.WorldServer.Engines.Pathfinder;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("npc_pos")]
    public class NpcPositionRecord : ActiveRecordBase<NpcPositionRecord>
    {

        public NpcPositionRecord()
        {
            this.Patterns = new Patterns.NpcPattern(this);
        }

        #region Db Fields

        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("NpcID")]
        public int NpcID
        {
            get;
            set;
        }

        [Property("MapId")]
        public int MapId
        {
            get;
            set;
        }

        [Property("CaseId")]
        public int CellId
        {
            get;
            set;
        }

        [Property("Orientation")]
        public int Orientation
        {
            get;
            set;
        }

        #endregion

        #region Methods and Fields

        public int TempID = -1;

        public Patterns.NpcPattern Patterns = null;

        public NpcRecord Template
        {
            get
            {
                return World.Helper.NpcHelper.GetTemplate(this.NpcID);
            }
        }

        public void APISpeak(string message)
        {
            World.Helper.MapHelper.FindMap(this.MapId).Engine.Send("cMK|" + this.TempID + "|" + this.Template.Name + "|" + message);
        }

        public void APISoloSpeak(World.Network.WorldClient player, string message)
        {
            player.Send("cMK|" + this.TempID + "|" + this.Template.Name + "|" + message);
        }

        public void APISoloEmote(World.Network.WorldClient player, int emote)
        {
            player.Send("eUK" + this.TempID + "|" + emote);
        }

        public void APISoloSmiley(World.Network.WorldClient player, int smileyID)
        {
            player.Send("cS" + this.TempID + "|" + smileyID);
        }

        public void APIEmote(int emote)
        {
            World.Helper.MapHelper.FindMap(this.MapId).Engine.Send("eUK" + this.TempID + "|" + emote);
        }

        public void APISmiley(int smileyID)
        {
            World.Helper.MapHelper.FindMap(this.MapId).Engine.Send("cS" + this.TempID + "|" + smileyID);
        }

        public void APIMove(int cellid)
        {
            var map = World.Helper.MapHelper.FindMap(this.MapId).Engine;
            var engine = new PathfindingV2(map);
            var path = engine.FindShortestPath(this.CellId, cellid, new List<int>());
            var intCells = new List<int>();
            path.ForEach(x => intCells.Add(x.ID));
            
            this.Patterns = new Patterns.NpcPattern(this);
            var strPath = Engines.Pathfinding.CreateStringPath(this.CellId, this.Orientation, intCells, map);
            map.Send("GA0;1;" + this.TempID + ";" + strPath);

            this.CellId = cellid;
        }

        #endregion


    }
}
