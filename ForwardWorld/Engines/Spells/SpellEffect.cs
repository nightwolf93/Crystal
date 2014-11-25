using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines.Spells
{
    public class SpellEffect
    {
        public SpellEngine Engine { get; set; }
        public string Data { get; set; }

        public Enums.SpellsEffects Effect = Enums.SpellsEffects.None;
        public int Value { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Turn { get; set; }
        public int Chance { get; set; }

        public string StrEffet { get; set; }
        public SpellTarget Targets { get; set; }

        public SpellEffect(SpellEngine engine, string data)
        {
            this.Engine = engine;
            this.Data = data;
            this.LoadEffect();
        }

        public override string ToString()
        {
            return "Value : " + Value + ", Value2 : " + Value2 + ", Value3 : " + Value3;
        }

        public void LoadEffect()
        {
            if (this.Data == "-1" || this.Data == "") return;
            try
            {
                string[] data = this.Data.Split(';');

                this.Effect = (Enums.SpellsEffects)int.Parse(data[0]);

                this.Value = int.Parse(data[1]);
                this.Value2 = int.Parse(data[2]);
                this.Value3 = int.Parse(data[3]);

                if (data.Length >= 5)
                {
                    this.Turn = int.Parse(data[4]);
                }

                if (data.Length >= 6)
                {
                    this.Chance = int.Parse(data[5]);
                }

                if (data.Length >= 7)
                {
                    this.StrEffet = data[6];
                }
                else
                {
                    this.StrEffet = "0d0+0";
                }

                if (data.Length >= 8)
                {
                    this.Targets = new SpellTarget(int.Parse(data[7]));
                }
                else
                {
                    this.Targets = new SpellTarget(23);
                }
            }
            catch (Exception e)
            {
                //Utilities.ConsoleStyle.Error("Cant load effect for spell '" + this.Engine.Spell.Name + "' : " + e.ToString());
            }
        }
    }
}
