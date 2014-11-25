using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountStatsEngine
*/

namespace Crystal.WorldServer.Engines
{
    public class MountStatsEngine
    {
        public List<Stats.MountStat> Stats = new List<Stats.MountStat>();

        public MountStatsEngine(string data)
        {
            string[] effects = data.Split('|');
            if (data != "")
            {
                foreach (string effect in effects)
                {
                    Stats.MountStat stat = new Stats.MountStat();
                    if (effect != "")
                    {
                        string[] eEffect = effect.Split(';');
                        foreach (string eParameters in eEffect)
                        {
                            if (eParameters != "")
                            {
                                string[] eParameterData = eParameters.Split('=');
                                string key = eParameterData[0].Trim().ToLower();
                                string value = eParameterData[1].Trim();
                                
                                switch (eParameterData[0])
                                {
                                    case "effectid":
                                        stat.EffectID = (Enums.ItemEffectEnum)int.Parse(value);
                                        break;

                                    case "coef":
                                        stat.Coef = double.Parse(value);
                                        break;

                                    case "value":
                                        stat.Value = double.Parse(value);
                                        break;
                                }
                            }
                        }
                    }
                    this.Stats.Add(stat);
                }
            }
        }

        public List<Stats.MountStat> RefreshStatForLevel(int level)
        {
            List<Stats.MountStat> rStats = new List<Stats.MountStat>();
            foreach (Stats.MountStat copyStat in this.Stats)
            {
                rStats.Add(new Stats.MountStat() { EffectID = copyStat.EffectID, Coef = copyStat.Coef, Value = copyStat.Value * level});
            }
            return rStats;
        }
    }
}
