using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines
{
    public class SpellEngine
    {
        public Database.Records.SpellRecord Spell { get; set; }

        public List<Spells.SpellLevel> Levels = new List<Spells.SpellLevel>();

        public SpellEngine(Database.Records.SpellRecord spell)
        {
            this.Spell = spell;
        }

        public void LoadLevels()
        {
            this.Levels.Add(new Spells.SpellLevel(1, Spell.Level1, this));
            this.Levels.Add(new Spells.SpellLevel(2, Spell.Level2, this));
            this.Levels.Add(new Spells.SpellLevel(3, Spell.Level3, this));
            this.Levels.Add(new Spells.SpellLevel(4, Spell.Level4, this));
            this.Levels.Add(new Spells.SpellLevel(5, Spell.Level5, this));
            this.Levels.Add(new Spells.SpellLevel(6, Spell.Level6, this));
        }

        public Spells.SpellLevel GetLevel(int level)
        {
            return this.Levels.FirstOrDefault(x => x.Level == level);
        }
    }
}
