using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crystal.WorldServer.World.Handlers.Items;
using Crystal.WorldServer.Enums;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines
{
    public class ItemEngine
    {
        private Database.Records.CharacterRecord _character;
        public List<Database.Records.WorldItemRecord> Items = new List<Database.Records.WorldItemRecord>();
        public List<Effect> SetsEffects = new List<Effect>();

        public List<Effect> Effects = new List<Effect>();

        public int CostInPa = 4;
        public int TauxCC = 30;
        public int TauxEC = 50;

        public ItemEngine() { }

        public ItemEngine(Database.Records.CharacterRecord character)
        {
            _character = character;
        }

        public void Load(string toParse, string weaPonInfos)
        {
            Effects = Parse(toParse);
            this.ParseWeaponInfos(weaPonInfos);
        }

        public bool IsWeaponEffect(int effectID)
        {
            switch (effectID)
            {
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                    return true;
                default:
                    return false;
            }
        }

        public Database.Records.WorldItemRecord DuplicateItem(Database.Records.WorldItemRecord item, int pos, bool send = true)
        {
            Database.Records.WorldItemRecord newItem = new Database.Records.WorldItemRecord();
            newItem.Effects = item.Effects;
            newItem.Template = item.Template;
            newItem.Position = pos;
            newItem.Quantity = 1;
            newItem.Owner = item.Owner;
            newItem.Engine.Load(newItem.Effects, item.GetTemplate.WeaponInfo);
            newItem.SaveAndFlush();
            if (send)
            {
                Items.Add(newItem);
                _character.Player.Send("OAKO" + newItem.DisplayItem);
                _character.Player.Action.RefreshPods();
            }
            return newItem;
        }

        public Database.Records.WorldItemRecord AddItem(Database.Records.WorldItemRecord item, bool newStack = false, int quantity = 1)
        {
            if (item.Position == -1)
            {
                try
                {
                dontHaveItem: ;
                    if (!HaveItemWithSameEffects(item.Effects, -1, item.Template) || newStack)
                    {
                        item.Quantity = quantity;
                        item.SaveAndFlush();
                        Items.Add(item);
                        if (_character != null)
                        {
                            if (_character.Player != null)
                            {
                                _character.Player.Send("OAKO" + item.DisplayItem);
                                _character.Player.Action.RefreshPods();
                            }
                        }
                        return item;
                    }
                    else
                    {
                        Database.Records.WorldItemRecord existItem = GetItemWithSameEffects(item.Effects, item.Template);
                        if (existItem != null)
                        {
                            item.DeleteAndFlush();
                            existItem.Quantity += quantity;
                            RefreshQuantity(existItem);
                            return existItem;
                        }
                        else
                        {
                            newStack = true;
                            goto dontHaveItem;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Cant add item : " + e.ToString());
                }
                return null;
            }
            else
            {
                if (_character != null)
                {
                    if (_character.Player != null)
                    {
                        _character.Player.Send("OAKO" + item.DisplayItem);
                        _character.Player.Action.RefreshPods();
                    }
                }
                Items.Add(item);
                return item;
            }
       }

        public void RemoveItem(Database.Records.WorldItemRecord item, int quantity, bool removeFromDB = true)
        {
            if (_character != null)
            {
                if (_character.Player != null)
                {
                    if (item.Quantity - quantity < 1)
                    {
                        _character.Player.Send("OR" + item.ID);
                        item.Owner = -1;
                        item.SaveAndFlush();
                        if(removeFromDB)
                            item.DeleteAndFlush();
                        Items.Remove(item);
                    }
                    else
                    {
                        item.Quantity -= quantity;
                        RefreshQuantity(item);
                    }
                    _character.Player.Action.RefreshPods();
                }
            }
        }

        public void RefreshQuantity(Database.Records.WorldItemRecord item)
        {
            if (_character != null)
            {
                if (_character.Player != null)
                {
                    _character.Player.Send("OQ" + item.ID + "|" + item.Quantity);
                }
            }
        }

        public bool HaveItemWithSameEffects(string effect, int itemID = -1, int templateID = -1)
        {
            if (templateID != -1)
            {
                return Items.FindAll(x => x.Effects == effect && x.ID != itemID && x.Template == templateID && x.Position == -1).Count > 0;
            }
            else
            {
                return Items.FindAll(x => x.Effects == effect && x.ID != itemID).Count > 0;
            }
        }

        public bool HaveItemID(int id)
        {
            return Items.FindAll(x => x.ID == id).Count > 0;
        }

        public bool HaveItem(int id)
        {
            return Items.FindAll(x => x.Template == id).Count > 0;
        }

        public Database.Records.WorldItemRecord GetItemWithSameEffects(string effect, int id, int itemID = -1)
        {
            return Items.FirstOrDefault(x => x.Effects == effect && x.Template == id && x.ID != itemID);
        }

        public Database.Records.WorldItemRecord GetItem(int id)
        {
            return Items.FirstOrDefault(x => x.ID == id);
        }

        public Database.Records.WorldItemRecord GetItemByTemplate(int id)
        {
            return Items.FirstOrDefault(x => x.Template == id);
        }

        public Database.Records.WorldItemRecord GetItemAtPos(int pos)
        {
            if(Items.FindAll(x => x.Position == pos).Count > 0)
            {
                return Items.FirstOrDefault(x => x.Position == pos);
            }
            else
            {
                return null;
            }
        }

        public List<Database.Records.WorldItemRecord> GetItemsStuffed()
        {
            return this.Items.FindAll(x => x.Position != -1);
        }

        public string DisplayItem()
        {
            string pattern = "";
            pattern += (GetItemAtPos(ItemPositionConstants.Arme) == null ? -900000 : GetItemAtPos(ItemPositionConstants.Arme).Template).ToString("x") + ",";
            pattern += (GetItemAtPos(ItemPositionConstants.Chapeau) == null ? -900000 : GetItemAtPos(ItemPositionConstants.Chapeau).Template).ToString("x") + ",";
            pattern += (GetItemAtPos(ItemPositionConstants.Cape) == null ? -900000 : GetItemAtPos(ItemPositionConstants.Cape).Template).ToString("x") + ",";
            pattern += (GetItemAtPos(ItemPositionConstants.Familier) == null ? -900000 : GetItemAtPos(ItemPositionConstants.Familier).Template).ToString("x") + ",";
            pattern += (GetItemAtPos(ItemPositionConstants.Bouclier) == null ? -900000 : GetItemAtPos(ItemPositionConstants.Bouclier).Template).ToString("x") + ",";
            return pattern.Replace("fff24460", "");
        }

        #region Effects

        public string StringEffect()
        {
            return string.Join(",", Effects);
        }

        public List<Effect> GetRandomEffect()
        {
            List<Effect> randomEffects = new List<Effect>();
            Effects.ForEach(x => randomEffects.Add(CreateRandomEffect(x)));
            return randomEffects;
        }

        public Effect CreateRandomEffect(Effect effect)
        {
            Effect newEffect = new Effect();
            newEffect.ID = effect.ID;
            if (IsWeaponEffect(effect.ID))
            {
                newEffect.Des.Min = effect.Des.Min + effect.Des.Fix;
                newEffect.Des.Fix = effect.Des.Fix;
                newEffect.Des.Max = effect.Des.Max + effect.Des.Fix;
            }
            else
            {
                newEffect.Des.Max = effect.Des.Max;
                newEffect.Des.Fix = Utilities.Basic.Rand(effect.Des.Min, effect.Des.Max) + effect.Des.Fix;
                newEffect.Des.Max = newEffect.Des.Fix;
            }
            return newEffect;
        }

        public void ParseWeaponInfos(string infos)
        {
            if (infos != "")
            {
                string[] data = infos.Split(',');
                this.CostInPa = int.Parse(data[1]);
                this.TauxCC = int.Parse(data[4]);
                this.TauxEC = int.Parse(data[5]);
            }
        }

        public static List<Effect> Parse(string toParse)
        {
            List<Effect> toReturn = new List<Effect>();
            foreach (string littleEffect in toParse.Split(','))
            {
                Effect newEffect = ParseLittleEffect(littleEffect);
                toReturn.Add(newEffect);
            }
            return toReturn;
        }

        public static Effect ParseLittleEffect(string littleEffect)
        {
            Effect e = new Effect();
            if (littleEffect != "")
            {
                string[] data = littleEffect.Split('#');
                if (data[0] != "-1")
                {
                    e.ID = Convert.ToInt32(data[0], 16);
                }
                if (data.Length > 1)
                {
                    if (data[1] != "")
                    {
                        e.Des.Min = Convert.ToInt32(data[1], 16);
                    }
                }
                if (data.Length > 2)
                {
                    if (data[2] != "")
                    {
                        e.Des.Max = Convert.ToInt32(data[2], 16);
                    }
                }
                if (data.Length > 4)
                {
                    if (data[4] != "")
                    {
                        if (data[4].Contains("+"))
                        {
                            string[] desEffect = data[4].Split('d');
                            e.Des.Min = int.Parse(desEffect[0]);
                            e.Des.Max = int.Parse(desEffect[1].Split('+')[0]);
                            e.Des.Fix = int.Parse(data[4].Split('+')[1]);
                        }
                    }
                }
            }
            return e;
        }

        #endregion

    }
}
