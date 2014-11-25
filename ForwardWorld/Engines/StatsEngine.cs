using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crystal.WorldServer.Engines.Stats;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines
{
    public class StatsEngine
    {
        private Database.Records.CharacterRecord _character;

        public SingleStats Life = new SingleStats();
        public SingleStats Wisdom = new SingleStats();
        public SingleStats Strenght = new SingleStats();
        public SingleStats Water = new SingleStats();
        public SingleStats Agility = new SingleStats();
        public SingleStats Fire = new SingleStats();

        public SingleStats PercentPointBonus = new SingleStats();
        public SingleStats FixPointBonus = new SingleStats();
        public SingleStats HealPointBonus = new SingleStats();
        public SingleStats ViewBonus = new SingleStats();
        public SingleStats CriticalBonus = new SingleStats();
        public SingleStats SummonBonus = new SingleStats() { Base = 1 };
        public SingleStats PodsBonus = new SingleStats();
        public SingleStats ReverseDamagesBonus = new SingleStats();

        public SingleStats ActionPoints = new SingleStats();
        public SingleStats MovementPoints = new SingleStats();

        public StatsEngine(Database.Records.MonsterLevelRecord monster)
        {
            this.Life.Base = monster.Life;
            this.ActionPoints.Base = monster.AP;
            this.MovementPoints.Base = monster.MP;
        }

        public StatsEngine(Database.Records.CharacterRecord character)
        {
            _character = character;
        }

        public int GetMaxActionPoints
        {
            get
            {
                if (_character == null)
                {
                    return ActionPoints.Total;
                }
                return 6 + ActionPoints.Total + (_character.Level >= 100 ? 1 : 0);
            }
        }

        public int GetMaxMovementPoints
        {
            get
            {
                if (_character == null)
                {
                    return MovementPoints.Total;
                }
                return 3 + MovementPoints.Total;
            }
        }

        public int MaxLife
        {
            get
            {
                if (_character == null)
                {
                    return Life.Total;
                }
                return 46 + (5 * _character.Level) + Life.Total;
            }
        }

        public int Initiative
        {
            get
            {
                return MaxLife * 2;
            }
        }

        public void RefreshStats()
        {
            if (_character != null)
            {
                _character.Player.Send("As" + _character.Pattern.CharacterStats);
            }
        }

        public void ResetBonus()
        {
            this.Agility.Bonus = 0;
            this.Life.Bonus = 0;
            this.Wisdom.Bonus = 0;
            this.Fire.Bonus = 0;
            this.Water.Bonus = 0;
            this.Strenght.Bonus = 0;
            this.ReverseDamagesBonus.Bonus = 0;
        }

        public SingleStats GetStats(int id)
        {
            switch ((Enums.StatsTypeEnum)id)
            {
                case Enums.StatsTypeEnum.Life:
                    return Life;

                case Enums.StatsTypeEnum.Wisdom:
                    return Wisdom;

                case Enums.StatsTypeEnum.Strenght:
                    return Strenght;

                case Enums.StatsTypeEnum.Fire:
                    return Fire;

                case Enums.StatsTypeEnum.Water:
                    return Water;

                case Enums.StatsTypeEnum.Agility:
                    return Agility;
            }
            return null;
        }

        public void ApplyEffect(World.Handlers.Items.Effect effect, bool remove = false)
        {
            //Console.WriteLine(effect.ID.ToString());
            int value = effect.Des.Fix;
            if (remove)
            {
                value = -value;
            }
            switch ((Enums.ItemEffectEnum)effect.ID)
            {
                case Enums.ItemEffectEnum.Strenght:
                    Strenght.Items += value;
                    break;

                case Enums.ItemEffectEnum.Wisdom:
                    Wisdom.Items += value;
                    break;

                case Enums.ItemEffectEnum.Life:
                    Life.Items += value;
                    this._character.CurrentLife += value;
                    break;

                case Enums.ItemEffectEnum.Fire:
                    Fire.Items += value;
                    break;

                case Enums.ItemEffectEnum.Agility:
                    Agility.Items += value;
                    break;

                case Enums.ItemEffectEnum.Water:
                    Water.Items += value;
                    break;

                case Enums.ItemEffectEnum.AP:
                    ActionPoints.Items += value;
                    break;

                case Enums.ItemEffectEnum.MP:
                    MovementPoints.Items += value;
                    break;

                case Enums.ItemEffectEnum.DamagePoint:
                    FixPointBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.DamagePercent:
                    PercentPointBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.HealPoint:
                    HealPointBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.ViewPoint:
                    ViewBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.CriticalPoint:
                    CriticalBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.SummonPoint:
                    SummonBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.AddPods:
                    PodsBonus.Items += value;
                    break;

                case Enums.ItemEffectEnum.ReverseDamage:
                    ReverseDamagesBonus.Items = value;
                    break;
            }
        }
    }
}
