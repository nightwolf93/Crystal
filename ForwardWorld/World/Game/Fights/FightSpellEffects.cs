using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Spells;

namespace Crystal.WorldServer.World.Game.Fights
{
    public static class FightSpellEffects
    {
        public static void ApplyEffect(Fight fight, Engines.Spells.SpellEffect effect, Engines.Spells.SpellLevel spellLevel, Fighter caster, int cellID,
                                    List<Fighter> targets)
        {
            if(Program.DebugMode) Utilities.ConsoleStyle.Debug("Used effect : " + effect.Effect.ToString() + "(" + spellLevel.TypeOfSpell.ToString() + ")");
            switch (effect.Effect)
            {
                case Enums.SpellsEffects.None:
                    Teleport(fight, caster, cellID);
                    break;

                case Enums.SpellsEffects.Teleport:
                    Teleport(fight, caster, cellID);
                    break;

                case Enums.SpellsEffects.DamageNeutre:
                    DirectDamages(fight, caster, targets, effect, 1);
                    break;

                case Enums.SpellsEffects.DamageLifeNeutre:
                    DirectLifeDamages(fight, caster, targets, effect);
                    break;

                case Enums.SpellsEffects.DamageTerre:
                    DirectDamages(fight, caster, targets, effect, 1);
                    break;

                case Enums.SpellsEffects.DamageFeu:
                    DirectDamages(fight, caster, targets, effect, 2);
                    break;

                case Enums.SpellsEffects.DamageEau:
                    DirectDamages(fight, caster, targets, effect, 3);
                    break;

                case Enums.SpellsEffects.DamageAir:
                    DirectDamages(fight, caster, targets, effect, 4);
                    break;

                case Enums.SpellsEffects.VolTerre:
                    StealLifeDamages(fight, caster, targets, effect, 1);
                    break;

                case Enums.SpellsEffects.VolFeu:
                    StealLifeDamages(fight, caster, targets, effect, 2);
                    break;

                case Enums.SpellsEffects.VolEau:
                    StealLifeDamages(fight, caster, targets, effect, 3);
                    break;

                case Enums.SpellsEffects.VolAir:
                    StealLifeDamages(fight, caster, targets, effect, 4);
                    break;

                case Enums.SpellsEffects.Heal:
                    Heal(fight, caster, targets, effect);
                    break;

                case Enums.SpellsEffects.AddPA:
                    AddAPBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddPM:
                    AddMPBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddVitalite:
                    AddLifeBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddDamagePercent:
                    AddDamagePercentBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddDamage:
                    AddDamageFixBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.SubPA:
                    SubAPBuff(fight, caster, effect, targets, false);
                    break;

                case Enums.SpellsEffects.SubPAEsquive:
                    SubAPBuff(fight, caster, effect, targets, true);
                    break;

                case Enums.SpellsEffects.SubPM:
                    SubMPBuff(fight, caster, effect, targets, false);
                    break;

                case Enums.SpellsEffects.SubPMEsquive:
                    SubMPBuff(fight, caster, effect, targets, true);
                    break;

                case Enums.SpellsEffects.SubPO:
                    SubPOBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddPO:
                    AddPOBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddDamageCritic:
                    AddCCBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddAgilite:
                    AddAgilityBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddForce:
                    AddStrenghtBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.DamageLife:
                    DirectLifeDamages(fight, caster, targets, effect);
                    break;

                case Enums.SpellsEffects.PushBack:
                    PushBack(fight, caster, targets, cellID, effect);
                    break;

                case Enums.SpellsEffects.PushFear:
                    PushFear(fight, caster, cellID, effect);
                    break;

                case Enums.SpellsEffects.PushFront:
                    PushFront(fight, caster, targets, cellID, effect);
                    break;

                case Enums.SpellsEffects.Transpose:
                    Transpose(fight, caster, targets, cellID, effect);
                    break;

                case Enums.SpellsEffects.MultiplyDamage:
                    AddDamagePercentBuff(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.UseTrap:
                    UseTrap(fight, caster, effect, spellLevel, cellID);
                    break;

                case Enums.SpellsEffects.AddCreature:
                    SummonCreature(fight, effect, spellLevel, caster, cellID);
                    break;

                case Enums.SpellsEffects.UseGlyph:
                    UseGlyph(fight, caster, effect, spellLevel, cellID);
                    break;

                case Enums.SpellsEffects.Invisible:
                    UseInvisibleState(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.UseCopyHuman:
                    SummonDouble(fight, effect, spellLevel, caster, cellID);
                    break;

                case Enums.SpellsEffects.ChangeSkin:
                    ChangeSkin(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.AddState:
                    AddState(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.LostState:
                    LostState(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.Porter:
                    Wear(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.Lancer:
                    LaunchWeared(fight, caster, effect, targets, cellID);
                    break;

                case Enums.SpellsEffects.AddChatiment:
                    AddChatiment(fight, caster, effect, targets, spellLevel);
                    break;

                case Enums.SpellsEffects.AddRenvoiDamage:
                    AddReverseDamage(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.LuckEcaflip:
                    AddLuckEcaflip(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddArmor:
                    AddArmor(fight, caster, effect, targets);
                    break;

                case Enums.SpellsEffects.AddReverseSpell:
                    AddReverseSpell(fight, caster, effect, spellLevel, targets);
                    break;
            }
        }

        #region Damages

        public static void DirectLifeDamages(Fight fight, Fighter caster, List<Fighter> targets, Engines.Spells.SpellEffect effect)
        {
            int damages = 0;
            int effectBase = effect.Value;
            damages = (caster.CurrentLife / 100) * effectBase;
            damages = (-damages);
            foreach (Fighter target in targets)
            {
                target.TakeDamages(target.ID, damages, 0);
            }
        }

        public static void DirectDamages(Fight fight, Fighter caster, List<Fighter> targets, Engines.Spells.SpellEffect effect, int element)
        {
            int damages = 0;
            int effectBase = effect.Value3;
            if (effect.Value > 0 && effect.Value2 > 0)
            {
                effectBase = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else if (effectBase < 0)
            {
                if (effect.Value >= effect.Value2)
                {
                    effectBase = effect.Value;
                }
                else
                {
                    effectBase = Utilities.Basic.Rand(effect.Value, effect.Value2);
                }
            }
            damages = RandomDamages(effectBase, caster, element);
            damages = (-damages);
            foreach (Fighter target in targets)
            {
                target.TakeDamages(caster.ID, damages, element);
            }
            caster.UnInvisible();
        }

        public static void StealLifeDamages(Fight fight, Fighter caster, List<Fighter> targets, Engines.Spells.SpellEffect effect, int element)
        {
            int damages = 0;
            int effectBase = effect.Value3;
            if (effect.Value > 0 && effect.Value2 > 0)
            {
                effectBase = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            damages = RandomDamages(effectBase, caster, element);
            int takenDamages = (-damages);
            int stealedLife = (int)Math.Truncate((double)(damages / 2));
            foreach (Fighter target in targets)
            {
                target.TakeDamages(target.ID, takenDamages, element);
                caster.Heal(caster.ID, stealedLife, element);
            }
            caster.UnInvisible();
        }

        public static void Heal(Fight fight, Fighter caster, List<Fighter> targets, Engines.Spells.SpellEffect effect)
        {
            int healing = 0;
            int effectBase = effect.Value3;
            if (effect.Value > 0 && effect.Value2 > 0)
            {
                effectBase = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            healing = RandomHeal(caster, effectBase);
            foreach (Fighter target in targets)
            {
                target.Heal(caster.ID, healing, 2);
            }
        }

        #endregion

        #region Floor

        public static void UseTrap(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, Engines.Spells.SpellLevel spellLevel, int cell)
        {
            fight.AddTrap(new FightTrap(caster, effect.Value, effect.Value2, spellLevel, cell, Engines.Pathfinding.GetDirNum(spellLevel.TypePO.Substring(1, 1)), Enums.FightTrapType.TRAP));
        }

        public static void UseGlyph(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, Engines.Spells.SpellLevel spellLevel, int cell)
        {
            fight.AddGlyph(new FightTrap(caster, effect.Value, effect.Value2, spellLevel, cell, Engines.Pathfinding.GetDirNum(spellLevel.TypePO.Substring(1, 1)), Enums.FightTrapType.GLYPH, effect.Value3, effect.Turn));
        }

        #endregion

        #region Positions

        public static void Teleport(Fight fight, Fighter caster, int cell)
        {
            if (fight.GetFighterOnCell(cell) == null)
            {
                fight.Send("GA0;4;" + caster.ID + ";" + caster.ID + "," + cell);
                caster.CellID = cell;
            }
        }

        public static void PushBack(Fight fight, Fighter caster, List<Fighter> targets, int cellID, Engines.Spells.SpellEffect effect)
        {
            int power = effect.Value;
            foreach (Fighter target in targets)
            {
                int dirPush = 0;
                if (target.CellID != cellID)
                {
                    dirPush = fight.Map.PathfindingMaker.GetDirection(cellID, target.CellID);
                }
                else
                {
                    dirPush = fight.Map.PathfindingMaker.GetDirection(caster.CellID, target.CellID);
                }

                int remoteCell = Engines.Pathfinding.GetRemoteCaseInThisDir(dirPush, power, target.CellID, fight.Map.Map);
                List<int> cellsPushed = Engines.Pathfinding.GetAllCellsForThisLinePath(dirPush, target.CellID, remoteCell, fight.Map.Map);

                if (target.ByWearFighter == null)
                {
                    foreach (int cell in cellsPushed)
                    {
                        if (target.IsDead)
                            break;
                        Fighter fighterOnCell = fight.GetFighterOnCell(cell);
                        if (fighterOnCell != null)
                        {
                            target.TakeDamages(target.ID, -(power * 10), 0);
                            fighterOnCell.TakeDamages(fighterOnCell.ID, -(power * 7), 0);
                            break;
                        }
                        if (!fight.Map.IsFree(cell))
                        {
                            target.TakeDamages(target.ID, -(power * 10), 0);
                            break;
                        }
                        target.CellID = cell;
                    }
                }
                if (target.WearedFighter != null)
                {
                    target.WearedFighter.CellID = target.CellID;
                }

                target.Team.Fight.Send("GA0;5;" + caster.ID + ";" + target.ID + "," + target.CellID);
            }

        }

        public static void PushFront(Fight fight, Fighter caster, List<Fighter> targets, int cellID, Engines.Spells.SpellEffect effect)
        {
            int power = effect.Value;
            foreach (Fighter target in targets)
            {
                int dirPush = 0;
                if (target.CellID != cellID)
                {
                    dirPush = fight.Map.PathfindingMaker.GetDirection(target.CellID, cellID);
                }
                else
                {
                    dirPush = fight.Map.PathfindingMaker.GetDirection(target.CellID, caster.CellID);
                }

                int remoteCell = Engines.Pathfinding.GetRemoteCaseInThisDir(dirPush, power, target.CellID, fight.Map.Map);
                List<int> cellsPushed = Engines.Pathfinding.GetAllCellsForThisLinePath(dirPush, target.CellID, remoteCell, fight.Map.Map);

                foreach (int cell in cellsPushed)
                {
                    Fighter fighterOnCell = fight.GetFighterOnCell(cell);
                    if (fighterOnCell != null) break;
                    if (!fight.Map.IsFree(cell)) break;
                    target.CellID = cell;
                }

                target.Team.Fight.Send("GA0;5;" + caster.ID + ";" + target.ID + "," + target.CellID);
            }

        }

        public static void PushFear(Fight fight, Fighter caster, int cellID, Engines.Spells.SpellEffect effect)
        {
            int dirPush = 0;

            if (caster.CellID != cellID)
            {
                dirPush = fight.Map.PathfindingMaker.GetDirection(caster.CellID, cellID);
            }
                else
            {
                dirPush = fight.Map.PathfindingMaker.GetDirection(caster.CellID, caster.CellID);
            }

            int nextCell = fight.Map.PathfindingMaker.NextCell(caster.CellID, dirPush);
            int power = fight.Map.PathfindingMaker.GetDistanceBetween(nextCell, cellID);

            Fighter target = fight.GetFighterOnCell(nextCell);
            if (target != null)
            {
                List<int> cellsPushed = Engines.Pathfinding.GetAllCellsForThisLinePath(dirPush, target.CellID, fight.Map.PathfindingMaker.NextCell(cellID, dirPush), fight.Map.Map);

                foreach (int cell in cellsPushed)
                {
                    Fighter fighterOnCell = fight.GetFighterOnCell(cell);
                    if (fighterOnCell != null) break;
                    if (!fight.Map.IsFree(cell)) break;
                    target.CellID = cell;
                }

                target.Team.Fight.Send("GA0;5;" + caster.ID + ";" + target.ID + "," + target.CellID);
            }
        }

        public static void Transpose(Fight fight, Fighter caster, List<Fighter> targets, int cellID, Engines.Spells.SpellEffect effect)
        {
            if (caster.CellID != cellID)
            {
                foreach (Fighter transposed in targets)
                {
                    int casterCell = caster.CellID;
                    int transposedCell = transposed.CellID;

                    caster.CellID = transposedCell;
                    transposed.CellID = casterCell;

                    fight.Send("GA0;4;" + caster.ID + ";" + caster.ID + "," + caster.CellID);
                    fight.Send("GA0;4;" + transposed.ID + ";" + transposed.ID + "," + transposed.CellID);
                }
            }
        }

        #endregion

        #region Buffs

        public static void AddAPBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedAP = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedAP = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedAP = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddPABuff(addedAP, effect.Turn, target), (int)effect.Effect, addedAP, true ,effect);
            }
        }

        public static void AddMPBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedMP = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedMP = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedMP = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddPMBuff(addedMP, effect.Turn, target), (int)effect.Effect, addedMP, true , effect);
            }
        }

        public static void AddLifeBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedAP = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedAP = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedAP = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddLifeBuff(addedAP, effect.Turn, target), (int)effect.Effect, addedAP, true, effect);
            }
        }

        public static void AddDamagePercentBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int damagesAdded = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                damagesAdded = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                damagesAdded = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddPercentDamage(damagesAdded, effect.Turn, target),
                    (int)effect.Effect, damagesAdded, true, effect);
            }
        }

        public static void AddDamageFixBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int damagesAdded = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                damagesAdded = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                damagesAdded = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddFixDamage(damagesAdded, effect.Turn, target),
                    (int)effect.Effect, damagesAdded, true, effect);
            }
        }

        public static void SubAPBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, bool canEsquiv)
        {
            int subAP = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                subAP = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                subAP = effect.Value;
            }

            foreach (Fighter target in targets)
            {
                int toRemove = subAP;
                int esquivedAp = 0;
                if (canEsquiv)
                {
                    for (int i = 0; i <= subAP - 1; i++)
                    {
                        bool esquiv = TryEsquiv(caster, target);
                        if (esquiv)
                        {
                            esquivedAp++;
                            toRemove--;
                        }
                    }
                }
                if (esquivedAp > 0)
                {
                    fight.Send("GA;308;" + caster.ID + ";" + target.ID + "," + esquivedAp);
                }
                target.AddBuff(caster.ID, new Spells.Buffs.SubPABuff(toRemove, effect.Turn, target), (int)effect.Effect, -toRemove, true, effect);
            }
        }

        public static void SubMPBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, bool canEsquiv)
        {
            int subMP = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                subMP = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                subMP = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                int toRemove = subMP;
                int esquivedAp = 0;
                if (canEsquiv)
                {
                    for (int i = 0; i <= subMP - 1; i++)
                    {
                        bool esquiv = TryEsquiv(caster, target);
                        if (esquiv)
                        {
                            esquivedAp++;
                            toRemove--;
                        }
                    }
                }
                if (esquivedAp > 0)
                {
                    fight.Send("GA;308;" + caster.ID + ";" + target.ID + "," + esquivedAp);
                }
                target.AddBuff(caster.ID, new Spells.Buffs.SubPMBuff(subMP, effect.Turn, target), (int)effect.Effect, -toRemove, true, effect);
            }
        }

        public static void AddPOBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedPO = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedPO = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedPO = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddPOBuff(addedPO, effect.Turn, target), (int)effect.Effect, addedPO, true, effect);
            }
        }

        public static void AddCCBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedCC = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedCC = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedCC = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddCriticalBuff(addedCC, effect.Turn, target), (int)effect.Effect, addedCC, true, effect);
            }
        }

        public static void SubPOBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int subPO = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                subPO = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                subPO = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.SubPOBuff(subPO, effect.Turn, target), (int)effect.Effect, subPO, true, effect);
            }
        }

        public static void AddAgilityBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedAgi = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedAgi = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedAgi = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddAgilityBuff(addedAgi, effect.Turn, target), (int)effect.Effect, addedAgi, true, effect);
            }
        }

        public static void AddStrenghtBuff(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            int addedStrenght = 0;
            if (effect.Value2 > 0 && effect.Value3 > 0)
            {
                addedStrenght = Utilities.Basic.Rand(effect.Value + effect.Value3, effect.Value2 + effect.Value3);
            }
            else
            {
                addedStrenght = effect.Value;
            }
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddStrenghtBuff(addedStrenght, effect.Turn, target), (int)effect.Effect, addedStrenght, true, effect);
            }
        }

        public static void DeleteAllBonus(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            foreach (Fighter target in targets)
            {
                target.RemoveAllBuffs();
                fight.Send("GA0;132;" + caster.ID + ";" + target.ID);
            }
        }

        public static void AddChatiment(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, Engines.Spells.SpellLevel spellLevel)
        {
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddChatiment(effect.Value2, effect.Value, 4, target, spellLevel), 0, effect.Value, true, effect);
            }
        }

        public static void AddReverseDamage(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            foreach (var target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddReverseDamage(effect.Value, effect.Turn, target), 0, effect.Value, true, effect);
            }
        }

        public static void AddLuckEcaflip(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            foreach (var target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddLuckEcaflipBuff(effect.Turn, target), 0, effect.Value, true, effect);
            }
        }

        #endregion

        #region Armor

        public static void AddArmor(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets)
        {
            foreach (var target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddArmorBuff(effect.Value, effect.Turn, target), 0, effect.Value, true, effect);
            }
        }

        public static void AddReverseSpell(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, SpellLevel spellLevel, List<Fighter> targets)
        {
            foreach (var target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.AddReverseSpellBuff(effect.Turn, spellLevel.Level, target), 0, effect.Value, true, effect);
            }
        }

        #endregion

        #region States

        public static void UseInvisibleState(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            foreach (Fighter target in targets)
            {
                target.AddBuff(caster.ID, new Spells.Buffs.InvisibleBuff(0, effect.Turn, target), (int)effect.Effect, 0, false, effect);
            }
        }

        public static void ChangeSkin(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            int skin = effect.Value3;
            foreach (Fighter target in targets)
            {
                if (effect.Value3 != -1)
                {
                    target.AddBuff(caster.ID, new Spells.Buffs.ChangeSkinBuff(skin, effect.Turn, target), (int)effect.Effect, 0, false, effect);
                    fight.Send("GA;149;" + caster.ID + ";" + target.ID + "," + skin + "," + skin + "," + effect.Turn);
                }
                else
                {
                    fight.Send("GA;149;" + caster.ID + ";" + target.ID +
                                "," + target.Look + "," + target.Look + ",-1");
                }
            }
        }

        public static void AddState(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            foreach (Fighter target in targets)
            {
                var buff = new Spells.Buffs.SaoulBuff(effect.Value3, effect.Turn, target);
                buff.StateType = (FighterState)effect.Value3;
                target.AddBuff(caster.ID, buff, (int)effect.Effect, 0, false, effect);
            }
        }

        public static void LostState(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            foreach (Fighter target in targets)
            {
                if (target.States.Contains((FighterState)effect.Value3))
                {
                    target.RemoveState((FighterState)effect.Value3);
                }
            }
        }

        public static void Wear(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            foreach (Fighter target in targets)
            {
                if (caster.WearedFighter == null)
                {
                    caster.Wear(target);
                }
            }
        }

        public static void LaunchWeared(Fight fight, Fighter caster, Engines.Spells.SpellEffect effect, List<Fighter> targets, int cellID)
        {
            if (fight.GetFighterOnCell(cellID) == null)
            {
                if (caster.WearedFighter != null)
                {
                    caster.WearedFighter.CellID = cellID;
                    caster.WearedFighter.UnWear();
                    fight.Send("GA0;51;" + caster.ID + ";" + cellID);
                }
                else
                {
                    fight.Send("GA;950;" + caster.ID + ";" + caster.ID + "," + (int)FighterState.Porteur + ",0");
                    caster.WearedFighter = null;
                    fight.Send("GA0;51;" + caster.ID + ";" + cellID);
                }
            }
        }

        #endregion

        #region Summon

        public static void SummonCreature(Fight fight, Engines.Spells.SpellEffect effect, Engines.Spells.SpellLevel spellLevel, Fighter caster, int cellID)
        {
            if (fight.GetFighterOnCell(cellID) == null)
            {
                Database.Records.MonstersTemplateRecord template = World.Helper.MonsterHelper.GetMonsterTemplate(effect.Value);
                if (template != null)
                {
                    Database.Records.MonsterLevelRecord level = template.Levels.FirstOrDefault(x => x.Level == spellLevel.Level);
                    if (level != null)
                    {
                        Fighter summonedCreature = new Fighter(fight.CurrentEntityTempID, level, null);
                        summonedCreature.CellID = cellID;
                        summonedCreature.SummonOwner = caster.ID;
                        summonedCreature.IsInvoc = true;
                        fight.CurrentEntityTempID--;
                        fight.AddPlayer(summonedCreature, caster.Team.ID, cellID);
                        fight.TimeLine.RemixTimeLine();
                        fight.TimelineDisplay();
                    }
                }
            }
        }

        public static void SummonDouble(Fight fight, Engines.Spells.SpellEffect effect, Engines.Spells.SpellLevel spellLevel, Fighter caster, int cellID)
        {
            /* Create temp template for double summoned */
            Database.Records.MonstersTemplateRecord tempTemplate = new Database.Records.MonstersTemplateRecord()
            {
                ID = -1,
                Color1 = caster.Character.Color1,
                Color2 = caster.Character.Color2,
                Color3 = caster.Character.Color3,
                Skin = caster.Character.Look,
                Name = caster.Nickname,
                Exp = 0,
                Kamas = "0,0",
                Drops = "",
                AI = 2,
            };
            Database.Records.MonsterLevelRecord tempLevel = new Database.Records.MonsterLevelRecord()
            {
                ID = -1,
                TemplateID = -1,
                IsTempLevel = true,
                TempTemplate = tempTemplate,
                Level = caster.Level,
                AP = caster.Stats.GetMaxActionPoints,
                MP = caster.Stats.GetMaxMovementPoints,
                Life = caster.Stats.MaxLife,
                Size = caster.Character.Scal,
                Stats = "0,0,0,0,0",
                ProtectStats = "0,0,0,0",
                Spells = "",
            };
            tempLevel.InitMonster();
            Fighter summonedCreature = new Fighter(fight.CurrentEntityTempID, tempLevel, null);
            summonedCreature.CellID = cellID;
            summonedCreature.SummonOwner = caster.ID;
            summonedCreature.IsInvoc = true;
            fight.CurrentEntityTempID--;
            fight.AddPlayer(summonedCreature, caster.Team.ID, cellID);
            fight.TimeLine.RemixTimeLine();
            fight.TimelineDisplay();
        }

        #endregion

        #region Methods

        public static int RandomHeal(Fighter fighter, int effectBase)
        {
            return (int)Math.Floor((double)(effectBase * (100 + fighter.Stats.Fire.Total) / 100 + fighter.Stats.HealPointBonus.Total));
        }

        public static int RandomDamages(int effectBase, Fighter fighter, int element)
        {
            int damages = effectBase;
            Engines.Stats.SingleStats stats = null;
            switch (element)
            {
                case 1://Force
                    stats = fighter.Stats.Strenght;
                    break;

                case 2://Fire
                    stats = fighter.Stats.Fire;
                    break;

                case 3://Water
                    stats = fighter.Stats.Water;
                    break;

                case 4://Agility
                    stats = fighter.Stats.Agility;
                    break;
            }
            damages = (int)Math.Floor((double)(effectBase * (100 + stats.Total +
                        fighter.Stats.PercentPointBonus.Total) / 100 + fighter.Stats.FixPointBonus.Total));
            return damages;
        }

        public static bool TryEsquiv(Fighter caster, Fighter target)
        {
            if (caster.Stats.Wisdom.Total > target.Stats.Wisdom.Total)
            {
                if (Utilities.Basic.Rand(0, caster.Stats.Wisdom.Total) + target.Stats.Wisdom.Total > caster.Stats.Wisdom.Total)
                {
                    return true;
                }
            }
            else
            {
                if (Utilities.Basic.Rand(0, target.Stats.Wisdom.Total) + caster.Stats.Wisdom.Total > target.Stats.Wisdom.Total)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
