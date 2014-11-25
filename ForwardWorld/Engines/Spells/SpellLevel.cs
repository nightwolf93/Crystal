using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines.Spells
{
    public class SpellLevel
    {
        public int Level { get; set; }
        public string Data { get; set; }

        public int CostPA { get; set; }
        public int MinPO { get; set; }
        public int MaxPO { get; set; }
        public int TauxCC { get; set; }
        public int TauxEC { get; set; }
        public bool InLine { get; set; }
        public bool NeedVisibility { get; set; }
        public bool NeedEmptyCell { get; set; }
        public bool POModifiable { get; set; }
        public int MaxPerTurn { get; set; }
        public int MaxPerPlayer { get; set; }
        public int TurnNumber { get; set; }
        public string TypePO { get; set; }
        public bool ECCanEndTurn { get; set; }

        public List<SpellEffect> Effects = new List<SpellEffect>();
        public List<SpellEffect> CriticalEffects = new List<SpellEffect>();

        public SpellEngine Engine { get; set; }

        public Enums.SpellTypeEnum TypeOfSpell = Enums.SpellTypeEnum.ATTACK;

        public SpellLevel(int level, string data, SpellEngine engine)
        {
            this.Level = level;
            this.Data = data;
            this.Engine = engine;
            this.LoadLevel();
            this.initSpellType();
        }

        public void LoadLevel()
        {
            try
            {
                if (this.Data == "-1" || this.Data == "")
                    return;
                string[] data = this.Data.Split(',');

                string basicEffects = data[1];
                string criticalEffects = data[0];

                this.CostPA = 6;
                if (Utilities.Basic.IsNumeric(data[2]))
                {
                    this.CostPA = int.Parse(data[2]);
                }

                this.MinPO = int.Parse(data[3]);
                this.MaxPO = int.Parse(data[4]);
                this.TauxCC = int.Parse(data[5]);
                this.TauxEC = int.Parse(data[6]);

                this.InLine = bool.Parse(data[7].Trim());
                this.NeedVisibility = bool.Parse(data[8].Trim());
                this.NeedEmptyCell = bool.Parse(data[9].Trim());
                this.POModifiable = bool.Parse(data[10].Trim());

                this.MaxPerTurn = int.Parse(data[12]);
                this.MaxPerPlayer = int.Parse(data[13]);
                this.TurnNumber = int.Parse(data[14]);
                this.TypePO = data[15].Trim();

                this.ECCanEndTurn = bool.Parse(data[19].Trim());

                basicEffects.Split('|').ToList().FindAll(x => x != "").ForEach(x => this.Effects.Add(new SpellEffect(Engine, x)));
                criticalEffects.Split('|').ToList().FindAll(x => x != "").ForEach(x => this.CriticalEffects.Add(new SpellEffect(Engine, x)));
            }
            catch (Exception e)
            {
                //Utilities.ConsoleStyle.Error("Cant load level '" + this.Level + "' for spell '" + this.Engine.Spell.Name + "'");
            }
        }

        private void initSpellType()
        {
            foreach (var e in this.Effects)
            {
                switch (e.Effect)
                {
                    case Enums.SpellsEffects.Teleport:
                        this.TypeOfSpell = Enums.SpellTypeEnum.MOVE;
                        break;

                    case Enums.SpellsEffects.AddDamage:
                    case Enums.SpellsEffects.AddDamagePhysic:
                    case Enums.SpellsEffects.AddDamagePercent:
                    case Enums.SpellsEffects.AddForce:
                    case Enums.SpellsEffects.Invisible:
                        this.TypeOfSpell = Enums.SpellTypeEnum.BOOST;
                        break;
                }
            }
        }
    }
}
