using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines
{
    public class SpellBook
    {
        private Database.Records.CharacterRecord _character;

        public List<World.Game.Spells.WorldSpell> Spells = new List<World.Game.Spells.WorldSpell>();

        public SpellBook(Database.Records.CharacterRecord character)
        {
            this._character = character;
        }

        public void AddSpell(string spell)
        {
            Spells.Add(new World.Game.Spells.WorldSpell(spell));
        }

        public void NewSpell(int spellID, int level, int position)
        {
            if (!HaveSpell(spellID))
            {
                Spells.Add(new World.Game.Spells.WorldSpell(spellID, level, position));
            }
        }

        public bool HaveSpell(int spellID)
        {
            return Spells.FindAll(x => x.SpellID == spellID).Count > 0;
        }

        public World.Game.Spells.WorldSpell GetSpellAtPos(int pos)
        {
            if (Spells.FindAll(x => x.Position == pos).Count > 0)
            {
                return Spells.FirstOrDefault(x => x.Position == pos);
            }
            else
            {
                return null;
            }         
        }

        public World.Game.Spells.WorldSpell GetSpell(int spellID)
        {
            return Spells.FirstOrDefault(x => x.SpellID == spellID);
        }

        public void LearnBaseSpell()
        {
            Database.Cache.BaseSpellCache.Cache.FindAll(x => x.Level <= _character.Level && x.Breed == _character.Breed).ForEach(x => NewSpell(x.SpellID, 1, x.Position));
        }

        public void SendSpells()
        {
            string packet = "SL";
            Spells.ForEach(x => packet += x.SpellID + "~" + x.Level + "~" + Pathfinding.GetDirChar(x.Position == -1 ? 25 : x.Position) + ";");
            this._character.Player.Send(packet);
        }
    }
}
