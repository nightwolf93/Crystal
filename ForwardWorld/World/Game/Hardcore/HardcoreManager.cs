using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Hardcore
{
    public class HardcoreManager
    {
        public static void DeathVersusMonster(List<Fights.Fighter> fighters, Engines.Map.MonsterGroup group)
        {
            foreach (var f in fighters)
            {
                if (f.IsHuman)//Check if is a human, because maybe contains summon
                {
                    DeathVersusMonster(f, group);
                }
            }
        }

        public static void DeathVersusMonster(Fights.Fighter fighter, Engines.Map.MonsterGroup group)
        {
            var skull = Helper.ItemHelper.GenerateItem(GetSkullBreedId(fighter.Character.Breed));

        }

        /// <summary>
        /// Get the skull id for the breed, this skull is added into the drop when the player die versus monsters or player
        /// </summary>
        /// <param name="breed">Breed ID</param>
        /// <returns>Skull Item ID</returns>
        public static int GetSkullBreedId(int breed)
        {
            switch (breed)
            {
                case 1: return 9077;
                case 2: return 9078;
                case 3: return 9079;
                case 4: return 9080;
                case 5: return 9081;
                case 6: return 9082;
                case 7: return 9083;
                case 8: return 9084;
                case 9: return 9085;
                case 10: return 9086;
                case 11: return 9087;
                case 12: return 9088;
                default: return -1;
            }
        }
    }
}
