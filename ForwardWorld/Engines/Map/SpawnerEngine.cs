using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines.Map
{
    public class SpawnerEngine
    {
        private MapEngine _map = null;

        private int _maxGroupOnMap = 3;
        private int _maxMonsterPerGroup = 8;

        public System.Timers.Timer SpawnTimer = new System.Timers.Timer(10000);

        public List<Database.Records.MonsterLevelRecord> PossibleMonsters = new List<Database.Records.MonsterLevelRecord>();

        public List<MonsterGroup> GroupsOnMap = new List<MonsterGroup>();

        public SpawnerEngine(MapEngine map)
        {
            this._map = map;
            this.SpawnTimer.Elapsed += new System.Timers.ElapsedEventHandler(SpawnTimer_Elapsed);
            this.SpawnTimer.Enabled = true;
            this.SpawnTimer.Start();
        }

        private void SpawnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            /* Group interaction */
            if (GroupsOnMap.Count > 0)
            {
                if (this._map.Players.CharactersOnMap.Count > 0)
                {
                    if (Utilities.ConfigurationManager.GetBoolValue("MonsterCanDoRandomMove"))
                    {
                        MonsterGroup randomMoveGroup = this.GroupsOnMap[Utilities.Basic.Rand(0, this.GroupsOnMap.Count - 1)];
                        int randomNextCell = Pathfinding.RandomJoinCell(randomMoveGroup.CellID, this._map.Map);
                        //On verifie si il marche sur une cellule valide et aussi qu'il ne pas sur un trigger
                        if (this._map.IsAvailableCell(randomNextCell) && this._map.Map.Triggers.FindAll(x => x.CellID == randomNextCell) == null)
                        {
                            string remakePath = Pathfinding.GetDirChar(randomMoveGroup.Dir) +
                                Pathfinding.GetCellChars(randomMoveGroup.CellID) +
                                Pathfinding.GetDirChar(this._map.PathfindingMaker.GetDirection
                                (randomMoveGroup.CellID, randomNextCell)) + Pathfinding.GetCellChars(randomNextCell);

                            this._map.Send("GA0;1;" + randomMoveGroup.ID + ";" + remakePath);

                            randomMoveGroup.Dir = this._map.PathfindingMaker.GetDirection
                                (randomMoveGroup.CellID, randomNextCell);
                            randomMoveGroup.CellID = randomNextCell;
                        }
                    }
                }
            }

            //if (GroupsOnMap.Count < _map.Map.MaximumGroup - 1)
            //{
            //    GenerateAllGroups();
            //    _map.Players.CharactersOnMap.ForEach(x => _map.ShowMonstersGroup(x));
            //}
        }

        public void SetRestriction(int maxGroupOnMap, int maxMonsterPerGroup)
        {
            this._maxGroupOnMap = maxGroupOnMap;
            this._maxMonsterPerGroup = maxMonsterPerGroup;

            //TODO : Refresh Maps
        }

        public void AddStringMonstersData(string data)
        {
            foreach (string monster in data.Split('|'))
            {
                if (monster != "")
                {
                    try
                    {
                        string[] monsterData = monster.Split(',');
                        int id = int.Parse(monsterData[0]);
                        int level = int.Parse(monsterData[1]);
                        Database.Records.MonstersTemplateRecord template = World.Helper.MonsterHelper.GetMonsterTemplate(id);
                        if (template != null)
                        {
                            Database.Records.MonsterLevelRecord templateLevel = template.GetLevel(level);
                            if (templateLevel != null)
                            {
                                this.PossibleMonsters.Add(templateLevel);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Can't add monster on map " + this._map.Map.ID + " : " + e.ToString());
                    }
                }
            }
        }

        public void AddPossibleMonster(int templateID)
        {

        }

        public Database.Records.MonsterLevelRecord GetRandomMonster()
        {
            if (this.PossibleMonsters.Count > 0)
            {
                return this.PossibleMonsters[Utilities.Basic.Rand(0, this.PossibleMonsters.Count - 1)];
            }
            else
            {
                return null;
            }
        }

        public void GenerateAllGroups()
        {
            for (int i = 0; i <= this._map.Map.MaximumGroup - 1; i++)
            {
                GenerateOneGroup();
            }
        }

        public void GenerateOneGroup()
        {
            try
            {
                MonsterGroup group = new MonsterGroup();
                if (this._map.Map.FixedGroup == 0)
                {
                    int groupSize = Utilities.Basic.Rand(1, this._maxMonsterPerGroup);
                    for (int i = 0; i <= groupSize - 1; i++)
                    {
                        Database.Records.MonsterLevelRecord monster = GetRandomMonster();
                        if (monster != null)
                        {
                            group.AddMonster(monster);
                        }
                    }
                }
                else
                {
                    foreach (var monster in this.PossibleMonsters)
                    {
                        group.AddMonster(monster);
                    }
                }

                group.ID = this._map.GetActorAvailableID;
                if (this._map.Emitters.Count > 0)
                {
                    group.CellID = this._map.Emitters[Utilities.Basic.Rand(0, this._map.Emitters.Count - 1)];
                }
                else
                {
                    group.CellID = this._map.RandomFreeCell().ID;
                }
                group.CreatePattern();
                this.GroupsOnMap.Add(group);
            }
            catch (Exception e)
            {
                //TODO
            }
        }
    }
}
