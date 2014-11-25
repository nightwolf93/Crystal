using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightShield
    {
        public Fighter Caster { get; set; }
        public Fighter ProtectedFighter { get; set; }
        public Engines.Spells.SpellLevel SpellLevel { get; set; }

        public List<int> ProtectedElements = new List<int>();

        public FightShield(Fighter caster, Fighter protectedFighter, Engines.Spells.SpellLevel spellLevel, int element)
        {
            this.Caster = caster;
            this.ProtectedFighter = protectedFighter;
            this.SpellLevel = spellLevel;
            this.ProtectedElements.Add(element);
        }

        public int GetReduceDamages(int element)
        {
            if (ProtectedElements.Contains(element))
            {
                //TODO: Formulas
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
