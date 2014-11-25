using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("characters")]
    public class CharacterRecord : ActiveRecordBase<CharacterRecord>
    {
        public CharacterRecord()
        {
            Stats = new Engines.StatsEngine(this);
            Items = new Engines.ItemEngine(this);
            Pattern = new Patterns.CharacterPattern(this);
            Spells = new Engines.SpellBook(this);
            Faction = new Engines.FactionEngine(this);
        }

        public void SetPlayer(World.Network.WorldClient client)
        {
            Player = client;
            if (Memory == null)
            {
                Memory = new Interop.Cache.PersistCache(Interop.Cache.PersistCache.CachePath + this.Nickname + ".character.cache");
            }
            if (GuildID != 0)
            {
                World.Game.Guilds.Guild guild =  World.Helper.GuildHelper.GetGuild(GuildID);
                if(guild != null)
                {
                    Player.Action.Guild = guild;
                    Player.Action.GuildMember = guild.FindMember(this.ID);
                }
            }
        }

        #region Database Fields

        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Owner")]
        public int Owner
        {
            get;
            set;
        }

        [Property("Nickname")]
        public string Nickname
        {
            get;
            set;
        }

        [Property("Level")]
        public int Level
        {
            get;
            set;
        }

        [Property("Experience")]
        public long Experience
        {
            get;
            set;
        }

        [Property("Look")]
        public int Look
        {
            get;
            set;
        }

        [Property("Scal")]
        public int Scal
        {
            get;
            set;
        }

        [Property("Gender")]
        public int Gender
        {
            get;
            set;
        }

        [Property("Breed")]
        public int Breed
        {
            get;
            set;
        }

        [Property("Color1")]
        public int Color1
        {
            get;
            set;
        }

        [Property("Color2")]
        public int Color2
        {
            get;
            set;
        }

        [Property("Color3")]
        public int Color3
        {
            get;
            set;
        }

        [Property("MapID")]
        public int MapID
        {
            get;
            set;
        }

        [Property("CellID")]
        public int CellID
        {
            get;
            set;
        }

        [Property("Direction")]
        public int Direction
        {
            get;
            set;
        }

        [Property("Kamas")]
        public int Kamas
        {
            get;
            set;
        }

        [Property("CurrentLife")]
        public int CurrentLife
        {
            get;
            set;
        }

        [Property("Life")]
        public int Life
        {
            get
            {
                return Stats.Life.Base;
            }
            set
            {
                Stats.Life.Base = value;
            }
        }

        [Property("Wisdom")]
        public int Wisdom
        {
            get
            {
                return Stats.Wisdom.Base;
            }
            set
            {
                Stats.Wisdom.Base = value;
            }
        }

        [Property("Strenght")]
        public int Strenght
        {
            get
            {
                return Stats.Strenght.Base;
            }
            set
            {
                Stats.Strenght.Base = value;
            }
        }

        [Property("Fire")]
        public int Fire
        {
            get
            {
                return Stats.Fire.Base;
            }
            set
            {
                Stats.Fire.Base = value;
            }
        }

        [Property("Agility")]
        public int Agility
        {
            get
            {
                return Stats.Agility.Base;
            }
            set
            {
                Stats.Agility.Base = value;
            }
        }

        [Property("Water")]
        public int Water
        {
            get
            {
                return Stats.Water.Base;
            }
            set
            {
                Stats.Water.Base = value;
            }
        }

        [Property("CaractPoint")]
        public int CaractPoint
        {
            get;
            set;
        }

        [Property("SpellPoint")]
        public int SpellPoint
        {
            get;
            set;
        }

        [Property("SaveMap")]
        public int SaveMap
        {
            get;
            set;
        }

        [Property("SaveCell")]
        public int SaveCell
        {
            get;
            set;
        }

        [Property("OwnZaaps")]
        public string OwnZaaps
        {
            get
            {
                return string.Join(",", Zaaps);
            }
            set
            {
                if (value == "")
                    return;
                string[] zaapsData = value.Split(',');
                zaapsData.ToList().ForEach(x => Zaaps.Add(int.Parse(x)));
            }
        }

        [Property("OwnSpells")]
        public string OwnSpells
        {
            get
            {
                return string.Join(";", Spells.Spells);
            }
            set
            {
                if (value != null && value != "")
                {
                    value.Split(';').ToList().ForEach(x => Spells.AddSpell(x));
                }          
            }
        }

        [Property("FactionID")]
        public int FactionID
        {
            get;
            set;
        }

        [Property("FactionPower")]
        public int FactionPower
        {
            get;
            set;
        }

        [Property("FactionHonor")]
        public int FactionHonor
        {
            get;
            set;
        }

        [Property("FactionDeshonor")]
        public int FactionDeshonor
        {
            get;
            set;
        }

        [Property("FactionEnabled")]
        public bool FactionEnabled
        {
            get;
            set;
        }

        [Property("GuildID")]
        public int GuildID
        {
            get;
            set;
        }

        [Property("GuildRank")]
        public int GuildRank
        {
            get;
            set;
        }

        [Property("GuildRights")]
        public string GuildRights
        {
            get;
            set;
        }

        [Property("MountID")]
        public int MountID
        {
            get;
            set;
        }

        [Property("RideMount")]
        public bool RideMount
        {
            get;
            set;
        }

        [Property("TitleID")]
        public int TitleID
        {
            get;
            set;
        }

        [Property("EliteLevel")]
        public int EliteLevel
        {
            get;
            set;
        }

        [Property("Jobs")]
        public string JobsData
        {
            get
            {
                return string.Join("|", this.Jobs);
            }
            set
            {
                foreach (var j in value.Split('|'))
                {
                    if (j != "")
                    {
                        var jdata = j.Split(',');
                        var job = World.Game.Jobs.JobManager.CreateNewJob((Enums.JobsIDEnums)int.Parse(jdata[0]));
                        job.Level = int.Parse(jdata[1]);
                        job.Experience = int.Parse(jdata[2]);
                        this.Jobs.Add(job);
                    }
                }
            }
        }

        #endregion

        #region Fields

        public MapRecords Map
        {
            get
            {
                if (Cache.MapCache.Cache.FindAll(x => x.ID == MapID).Count > 0)
                {
                    return Cache.MapCache.Cache.First(x => x.ID == MapID);
                }
                else
                {
                    return null;
                }
            }
        }
        public ExpFloorRecord ExpFloor
        {
            get
            {
                return World.Helper.ExpFloorHelper.GetCharactersFloor(this.Experience);
            }
        }
        public WorldMountRecord Mount
        { 
            get; 
            set;
        }

        public Patterns.CharacterPattern Pattern
        {
            get;
            set;
        }

        public Engines.StatsEngine Stats
        {
            get;
            set;
        }
        public Engines.ItemEngine Items
        {
            get;
            set;
        }
        public Engines.PartyEngine Party
        {
            get;
            set;
        }
        public Engines.SpellBook Spells
        {
            get;
            set;
        }
        public Engines.FactionEngine Faction { get; set; }

        public Interop.Cache.PersistCache Memory { get; set; }

        public World.Network.WorldClient Player
        {
            get;
            set;
        }

        public World.Game.Fights.Fighter Fighter
        {
            get;
            set;
        }

        public List<Database.Records.ItemSetRecord> StuffedSets
        {
            get
            {
                var sets = new List<Database.Records.ItemSetRecord>();
                foreach (var stuff in this.Items.GetItemsStuffed())
                {
                    if (stuff.GetTemplate != null)
                    {
                        if (stuff.GetTemplate.Set != null)
                        {
                            if (!sets.Contains(stuff.GetTemplate.Set))
                            {
                                sets.Add(stuff.GetTemplate.Set);
                            }
                        }
                    }
                }
                return sets;
            }
        }

        public int GetStuffedItemSetCount(int set)
        {
            var count = 0;
            foreach (var stuff in this.Items.GetItemsStuffed())
            {
                if (stuff.GetTemplate != null)
                {
                    if (stuff.GetTemplate.Set != null)
                    {
                        if (stuff.GetTemplate.Set.ID == set)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public List<int> GetStuffedItemSet(int set)
        {
            var items = new List<int>();
            foreach (var stuff in this.Items.GetItemsStuffed())
            {
                if (stuff.GetTemplate != null)
                {
                    if (stuff.GetTemplate.Set != null)
                    {
                        if (stuff.GetTemplate.Set.ID == set)
                        {
                            items.Add(stuff.GetTemplate.ID);
                        }
                    }
                }
            }
            return items;
        }

        public bool FirstConnection = true;
        public bool FirstPlay = false;
        public int NextMove = 0;
        public List<int> Zaaps = new List<int>();
        public List<World.Handlers.Items.Effect> SetEffects = new List<World.Handlers.Items.Effect>();
        public List<World.Game.Jobs.Job> Jobs = new List<World.Game.Jobs.Job>();

        #endregion

        public override void Save()
        {
            this.Memory.Save();
            base.Save();
        }

        public Database.Records.WorldItemRecord AddItem(Database.Records.WorldItemRecord item, int quantity)
        {
            if (World.Helper.ItemHelper.CanCreateStack(item.Template))
            {
                return this.Items.AddItem(item, false, quantity);
            }
            else
            {
                return this.Items.AddItem(item, true, quantity);
            }
        }

        /// <summary>
        /// Refresh the stats of the character itemset
        /// </summary>
        public void RefreshItemSet()
        {
            //Remove set effect
            foreach (var e in this.SetEffects)
            {
                this.Stats.ApplyEffect(e, true);
            }
            this.SetEffects.Clear();

            foreach (var set in this.StuffedSets)
            {
                var itemStuffed = this.GetStuffedItemSetCount(set.ID);
                if (set.BonusList.ContainsKey(itemStuffed))
                {
                    foreach (var e in set.BonusList[itemStuffed])
                    {
                        this.SetEffects.Add(e);
                        this.Stats.ApplyEffect(e);
                    }

                    //Client.Send("OS+" & PanoId & "|" & ListItem & "|" & EffectString)
                    if (this.Player != null)
                    {
                        this.Player.Send("OS+" + set.ID + "|" + string.Join(";", this.GetStuffedItemSet(set.ID)) + "|" + set.GetEffect(itemStuffed));
                    }
                }
            }
        }

        #region Job

        public bool HaveJob(Enums.JobsIDEnums id)
        {
            return this.Jobs.FindAll(x => x.JobID == (int)id).Count > 0;
        }

        public World.Game.Jobs.Job GetJobBySkill(int skill)
        {
            foreach (var job in this.Jobs)
            {
                if (job.Skills.FindAll(x => x.ID == skill).Count > 0)
                {
                    return job;
                }
            }
            return null;
        }

        #endregion

        #region Breed

        //public BreedRecord GetBreed()
        //{
        //    return Database.Cache.BreedCache.
        //}

        #endregion
    }
}
