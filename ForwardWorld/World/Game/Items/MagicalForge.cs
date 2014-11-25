using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MagicalForge
*/

namespace Crystal.WorldServer.World.Game.Items
{
    public static class MagicalForge
    {

        /// <summary>
        /// TEMP METHOD
        /// </summary>
        public static void FmMyWeapon(Network.WorldClient client, Database.Records.WorldItemRecord item, int type)
        {
            foreach (var effect in item.Engine.Effects)
            {
                if (effect.ID == 100)
                {
                    switch (type)
                    {
                        case 1://Force
                            effect.ID = 97;
                            break;

                        case 2://feu
                            effect.ID = 99;
                            break;

                        case 3://chance
                            effect.ID = 96;
                            break;

                        case 4://agi
                            effect.ID = 98;
                            break;
                    }
                }
            }
        }
    }
}
