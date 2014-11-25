using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountStat
*/

namespace Crystal.WorldServer.Engines.Stats
{
    public class MountStat
    {
        public Enums.ItemEffectEnum EffectID { get; set; }
        public double Coef;
        public double Value;

        public override string ToString()
        {
            return ((int)this.EffectID).ToString("x") + "#" + ((int)Value).ToString("x") + "#0#0";
        }
    }
}
