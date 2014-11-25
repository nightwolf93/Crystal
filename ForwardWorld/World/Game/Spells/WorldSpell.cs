using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Game.Spells
{
    public class WorldSpell
    {
        public int SpellID;
        public int Level;
        public int Position;

        public Database.Records.SpellRecord Template
        {
            get
            {
                return Database.Cache.SpellCache.Cache.FirstOrDefault(x => x.ID == SpellID);
            }
        }

        public WorldSpell(int spellID, int level, int position)
        {
            this.SpellID = spellID;
            this.Level = level;
            this.Position = position;
        }

        public WorldSpell(string spell)
        {
            try
            {
                string[] data = spell.Split(',');
                this.SpellID = int.Parse(data[0]);
                this.Level = int.Parse(data[1]);
                this.Position = int.Parse(data[2]);
            }
            catch { }
        }

        public override string ToString()
        {
            return SpellID + "," + Level + "," + Position;
        }
    }
}
