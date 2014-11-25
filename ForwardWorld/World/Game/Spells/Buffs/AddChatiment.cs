using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddChatiment : Engines.Spells.SpellBuff
    {
        public int Value = 0;
        public int CurrentAdded = 0;
        public int Element = 0;
        public Engines.Spells.SpellLevel Spell { get; set; }

        public AddChatiment(int value, int element, int duration, Fights.Fighter fighter, Engines.Spells.SpellLevel spell)
            : base(duration, true, fighter) 
        {
            this.Element = element;
            this.Value = value;
            this.Spell = spell;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.Stats.CriticalBonus.Bonus += Value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.Stats.CriticalBonus.Bonus -= Value;
        }

        public override void FighterHit(int damages)
        {
            if (CurrentAdded >= Value)
                return;

            int amount = +damages;
            if (CurrentAdded + amount >= Value)
            {
                amount = 0;
            }

            amount = -amount;

            var effect = new Engines.Spells.SpellEffect(this.Spell.Engine, this.Spell.Data);
            switch ((Enums.SpellsEffects)Element)
            {
                case Enums.SpellsEffects.AddAgilite:
                    BuffedFighter.AddBuff(BuffedFighter.Character.ID, new AddAgilityBuff(amount, 4, BuffedFighter), 0, amount, true, effect);
                    break;

                case Enums.SpellsEffects.AddForce:
                    BuffedFighter.AddBuff(BuffedFighter.Character.ID, new AddStrenghtBuff(amount, 4, BuffedFighter), 0, amount, true, effect);
                    break;

                case Enums.SpellsEffects.AddIntelligence:
                    BuffedFighter.AddBuff(BuffedFighter.Character.ID, new AddFireBuff(amount, 4, BuffedFighter), 0, amount, true, effect);
                    break;

                case Enums.SpellsEffects.AddChance:
                    BuffedFighter.AddBuff(BuffedFighter.Character.ID, new AddWaterBuff(amount, 4, BuffedFighter), 0, amount, true, effect);
                    break;
            }
            CurrentAdded += amount;
            base.FighterHit(damages);
        }

        public void Refresh()
        {
            this.CurrentAdded = 0;
        }
    }
}
