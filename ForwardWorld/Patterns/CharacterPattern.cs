using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Patterns
{
    public class CharacterPattern
    {
        private Database.Records.CharacterRecord _character;

        public CharacterPattern(Database.Records.CharacterRecord character)
        {
            this._character = character;
        }

        public string ShowCharacterOnMap
        {
            get
            {
                try
                {
                    StringBuilder pattern = new StringBuilder();
                    pattern.Append(_character.CellID).Append(";").Append(_character.Direction).Append(";0;").Append(_character.ID).Append(";")
                        .Append(_character.Nickname).Append(";").Append(_character.Breed.ToString())
                        .Append(_character.TitleID > 0 ? "," + _character.TitleID : "").Append(";").Append(_character.Look).Append("^").Append(_character.Scal).Append(";")
                        .Append(_character.Gender).Append(";").Append(_character.Faction.Wings).Append(",").Append((_character.ID + _character.Level).ToString()).Append(";")
                        .Append(_character.Color1.ToString("x")).Append(";").Append(_character.Color2.ToString("x")).Append(";")
                        .Append(_character.Color3.ToString("x")).Append(";")
                        .Append(_character.Items.DisplayItem()).Append(";" + Aura + ";;;" + GuildInfos + ";0;" + ShowMount + ";");
                    return pattern.ToString();
                }
                catch (Exception e)
                {
                    return "";
                }
            }
        }

        public string GuildInfos
        {
            get
            {
                if (_character.Player.Action.Guild != null)
                {
                    if (!Utilities.ConfigurationManager.GetBoolValue("SkipGuildsRestrictions"))
                    {
                        if (!_character.Player.Action.Guild.HaveRequiredMembers)
                        {
                            return ";";
                        }
                    }
                    
                    return _character.Player.Action.Guild.Name + ";" +
                            _character.Player.Action.Guild.DisplayEmblemPattern;
                }
                else
                {
                    return ";";
                }
            }
        }

        public string ShowCharacterInParty
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(_character.ID).Append(";").Append(_character.Nickname).Append(";").Append(_character.Look).Append(";")
                    .Append(_character.Color1.ToString("x")).Append(";").Append(_character.Color2.ToString("x")).Append(";")
                    .Append(_character.Color3.ToString("x")).Append(";").Append(_character.Items.DisplayItem()).Append(";").Append(_character.CurrentLife)
                    .Append(",").Append(_character.Stats.MaxLife).Append(";").Append(_character.Level).Append(";")
                    .Append("0;0;0");
                return pattern.ToString();
            }
        }

        public string ShowCharacterInBattle
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(_character.Fighter.CellID).Append(";1;0;").Append(_character.ID.ToString());
                pattern.Append(";").Append(_character.Nickname).Append(";").Append(_character.Breed).Append(";");
                pattern.Append(_character.Look).Append("^").Append(_character.Scal).Append(";").Append(_character.Gender);
                pattern.Append(";").Append(_character.Level.ToString()).Append(";").Append(";");//Alignement
                pattern.Append(_character.Color1.ToString("x")).Append(";").Append(_character.Color2.ToString("x")).Append(";")
                    .Append(_character.Color3.ToString("x")).Append(";");
                pattern.Append(_character.Items.DisplayItem()).Append(";").Append(_character.CurrentLife).Append(";");
                pattern.Append(_character.Stats.GetMaxActionPoints.ToString()).Append(";").Append(_character.Stats.GetMaxMovementPoints.ToString());
                pattern.Append(";0;0;0;0;0;0;0;").Append(_character.Fighter.Team.ID.ToString()).Append(";" + ShowMount + ";");
                return pattern.ToString();
            }
        }

        public string ShowMountPanel
        {
            get
            {
                StringBuilder pattern = new StringBuilder("Re+");
                pattern.Append(_character.Mount.GetMountData);
                return pattern.ToString();
            }
        }

        public string ShowMount
        {
            get
            {
                if (_character.Mount != null)
                {
                    if (_character.RideMount)
                    {
                        return _character.Mount.MountType.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public int Aura
        {
            get
            {
                if (_character.Level >= 100)
                {
                    if (Utilities.ConfigurationManager.GetBoolValue("EnableAura"))
                    {
                        if (_character.Level >= 200)
                        {
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public string CharacterStats
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(_character.Experience + "," + _character.ExpFloor.Character + ",");//TODO: Exp !
                pattern.Append(World.Helper.ExpFloorHelper.GetNextCharactersLevelFloor(_character.Level) == null ? "-1" : World.Helper.ExpFloorHelper.GetNextCharactersLevelFloor(_character.Level).Character.ToString());
                pattern.Append("|" + _character.Kamas + "|" + _character.CaractPoint + "|" + _character.SpellPoint);
                pattern.Append("|"+ _character.Faction.FactionStats);
                pattern.Append("|" + _character.CurrentLife + "," + _character.Stats.MaxLife);
                pattern.Append("|10000,10000");
                pattern.Append("|0|0");
                pattern.Append("|" + _character.Stats.ActionPoints.ToString());
                pattern.Append("|" + _character.Stats.MovementPoints.ToString());
                pattern.Append("|" + _character.Stats.Strenght.ToString());
                pattern.Append("|" + _character.Stats.Life.ToString());
                pattern.Append("|" + _character.Stats.Wisdom.ToString());
                pattern.Append("|" + _character.Stats.Water.ToString());
                pattern.Append("|" + _character.Stats.Agility.ToString());
                pattern.Append("|" + _character.Stats.Fire.ToString());
                pattern.Append("|" + _character.Stats.ViewBonus.ToString());
                pattern.Append("|" + _character.Stats.SummonBonus.ToString());
                pattern.Append("|" + _character.Stats.FixPointBonus.ToString());
                pattern.Append("|0,0,0,0");
                pattern.Append("|0,0,0,0");
                pattern.Append("|" + _character.Stats.PercentPointBonus.ToString());
                pattern.Append("|" + _character.Stats.HealPointBonus.ToString());
                pattern.Append("|0,0,0,0");
                pattern.Append("|0,0,0,0");
                pattern.Append("|0,0,0,0");
                pattern.Append("|" + _character.Stats.CriticalBonus.ToString());
                pattern.Append("|0,0,0,0");
                return pattern.ToString();
            }
        }

        public string CharacterToEnemiesListKnow
        {
            get
            {
                StringBuilder packet = new StringBuilder();
                packet.Append(";?;");//FIXME
                packet.Append(_character.Nickname).Append(";");
                packet.Append(_character.Level).Append(";");
                packet.Append("-1;");//FIXME : Alignement
                packet.Append(_character.Breed).Append(";");
                packet.Append(_character.Gender).Append(";");
                packet.Append(_character.Look).Append(";");
                return packet.ToString();
            }
        }

        public string CharacterToEnemiesListUnKnow
        {
            get
            {
                StringBuilder packet = new StringBuilder();
                packet.Append(";?;");//FIXME
                packet.Append(_character.Nickname).Append(";");
                packet.Append("?").Append(";");
                packet.Append("-1;");//FIXME : Alignement
                packet.Append(_character.Breed).Append(";");
                packet.Append(_character.Gender).Append(";");
                packet.Append(_character.Look).Append(";");
                return packet.ToString();
            }
        }

        public string CharacterToFriendsListKnow
        {
            get
            {
                StringBuilder packet = new StringBuilder();
                packet.Append(";?;");//FIXME
                packet.Append(_character.Nickname).Append(";");
                packet.Append(_character.Level).Append(";");
                packet.Append("-1;");//FIXME : Alignement
                packet.Append(_character.Breed).Append(";");
                packet.Append(_character.Gender).Append(";");
                packet.Append(_character.Look).Append(";");
                return packet.ToString();
            }
        }

        public string CharacterToFriendsListUnKnow
        {
            get
            {
                StringBuilder packet = new StringBuilder();
                packet.Append(";?;");//FIXME
                packet.Append(_character.Nickname).Append(";");
                packet.Append("?").Append(";");
                packet.Append("-1;");//FIXME : Alignement
                packet.Append(_character.Breed).Append(";");
                packet.Append(_character.Gender).Append(";");
                packet.Append(_character.Look).Append(";");
                return packet.ToString();
            }
        }
    }
}
