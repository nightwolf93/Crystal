using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

using Crystal.WorldServer.Engines.Path;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines
{
    public class MapEngine
    {
        public Database.Records.MapRecords Map;
        public bool _cellsInit = false;
        private int _currentTempId = -1;

        public Dictionary<int, Cell> Cells = new Dictionary<int,Cell>();
        public List<int> Emitters = new List<int>();
        public Dictionary<int, List<int>> Places = new Dictionary<int, List<int>>();

        /* Complementary Engines */
        public Map.SpawnerEngine Spawner;
        public Map.PlayersMapEngine Players;
        public Pathfinding PathfindingMaker;

        private Cell[,] mMatrix = null;

        /* Generic lists */
        public List<Database.Records.NpcPositionRecord> Npcs = new List<Database.Records.NpcPositionRecord>();
        public List<World.Game.Fights.Fight> FightsOnMap = new List<World.Game.Fights.Fight>();
        public List<Database.Records.PaddockRecord> Paddocks = new List<Database.Records.PaddockRecord>();
        public Dictionary<int, Database.Records.WorldItemRecord> DroppedItems = new Dictionary<int, Database.Records.WorldItemRecord>();
        public Dictionary<int, World.Game.IO.InteractiveObject> InteractiveObjects = new Dictionary<int, World.Game.IO.InteractiveObject>();

        public MapEngine(Database.Records.MapRecords map)
        {
            Map = map;
            PathfindingMaker = new Pathfinding("", this.Map, 0, 0);
            Spawner = new Map.SpawnerEngine(this);
            Players = new Map.PlayersMapEngine(this);

        }

        #region Display

        public void ShowMap(World.Network.WorldClient client)
        {
            if (Map.DecryptKey != "")
            {
                client.Send(new StringBuilder("GDM|").Append(Map.ID)
                    .Append("|").Append(Map.CreateTime).Append("|").Append(Map.DecryptKey).ToString());
            }
            else
            {
                client.Send(new StringBuilder("GDM|").Append(Map.ID)
                              .Append("|").Append(Map.CreateTime).ToString());
            }
        }

        public void ShowNpcsOnMap(World.Network.WorldClient client)
        {
            try
            {
                string globalPattern = "GM";
                foreach (var npc in Npcs)
                {
                    try
                    {
                        globalPattern += npc.Patterns.DisplayOnMap;
                    }
                    catch (Exception e)
                    {

                    }
                }
                client.Send(globalPattern);
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't show npc : " + e.ToString());
            }
        }

        public void ShowMonstersGroup(World.Network.WorldClient client)
        {
            try
            {
                this.Spawner.GroupsOnMap.ForEach(x => client.Send("GM" + x.CatchedPattern));
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't show fights : " + e.ToString());
            }
        }

        public void ShowAllFightOnMap(World.Network.WorldClient client)
        {
            try
            {
                this.FightsOnMap.FindAll(x => x.State == World.Game.Fights.Fight.FightState.PlacementsPhase)
                                                            .ForEach(x => ShowFight(x, client));
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't show fights : " + e.ToString());
            }
        }

        public void ShowFight(World.Game.Fights.Fight fight, World.Network.WorldClient client)
        {
            client.Send("Gc+" + fight.DisplayPatternBlades);
            client.Send("Gt" + fight.RedTeam.DisplayPatternBladeTeam);
            client.Send("Gt" + fight.BlueTeam.DisplayPatternBladeTeam);
        }

        public void ShowFight(World.Game.Fights.Fight fight)
        {
            this.Send("Gc+" + fight.DisplayPatternBlades);
            this.Send("Gt" + fight.RedTeam.DisplayPatternBladeTeam);
            this.Send("Gt" + fight.BlueTeam.DisplayPatternBladeTeam);
        }

        public void HideFight(World.Game.Fights.Fight fight)
        {
            this.Send("Gc-" + fight.ID);
        }

        public void ShowDroppedItem(Database.Records.WorldItemRecord item, int cellid)
        {
            this.Send("GDO+"  + cellid + ";" + item.Template + ";" + 0);
        }

        public void UnShowDroppedItem(int cellid)
        {
            this.Send("GDO-" + cellid);
        }

        public void ShowDroppedItem(Database.Records.WorldItemRecord item, int cellid, World.Network.WorldClient client)
        {
            client.Send("GDO+" + cellid + ";" + item.Template + ";" + 0);
        }

        public void ShowAllDroppedItems(World.Network.WorldClient client)
        {
            try
            {
                foreach (KeyValuePair<int, Database.Records.WorldItemRecord> item in this.DroppedItems)
                {
                    ShowDroppedItem(item.Value, item.Key, client);
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't show dropped item : " + e.ToString());
            }
        }

        public void ShowFightCountOnMap(World.Network.WorldClient client)
        {
            client.Send("fC" + this.FightsOnMap.Count);
        }

        public void ShowFightCountOnMap()
        {
            this.Send("fC" + this.FightsOnMap.Count);
        }

        public void ShowFightListInfos(World.Network.WorldClient client)
        {
            string packet = "fL";
            foreach (World.Game.Fights.Fight fight in FightsOnMap)
            {
                if (packet.Length > 2)
                {
                    packet += "|";
                }
                packet += fight.DisplayFightInformations;
            }
            client.Send(packet);
        }

        public void ShowPaddocks(World.Network.WorldClient client)
        {
            foreach (var paddock in this.Paddocks)
            {
                string packet = "Rp";
                if (paddock.IsPublic)
                {
                    packet += "-1;";
                    packet += "0;";
                    packet += paddock.Capacity + ";";
                    packet += "0;";
                }
                else
                {
                    packet += paddock.Owner + ";";
                    packet += paddock.Cost + ";";
                    packet += paddock.Capacity + ";";
                    packet += "0;";
                }
                client.Send(packet);
            }
        }

        public void ShowPaddocksMounts(World.Network.WorldClient client, int cellid = -1)
        {
            if (cellid == -1)
            {
                World.Handlers.MountHandler.ShowPaddocksMountData(client);
                return;
            }
            Database.Records.PaddockRecord paddock = GetPaddock(cellid);
            if (paddock != null)
            {
                World.Handlers.MountHandler.ShowPaddocksMountData(client);
            }
        }

        public void ShowIO(World.Network.WorldClient client)
        {
            if (!Program.ARKALIA)
            {
                var packet = new StringBuilder();
                foreach (var io in this.InteractiveObjects)
                {
                    packet.Append(io.Value.GetPattern()).Append("\x00");
                }
                client.Send(packet.ToString());
            }
        }

        #endregion

        #region Data

        public void InitContents()
        {
            this.InitEmitters();
            this.InitPlaces();
        }

        public void InitEmitters()
        {
            foreach (string emitter in this.Map.SpawnEmitters.Split(',').ToList())
            {
                if (emitter != null)
                {
                    if (emitter != "")
                    {
                        Emitters.Add(int.Parse(emitter));
                    }
                }
            }
        }

        public void SetPlaces()
        {
            if(this.Places.Count == 0)
            {
                this.Places.Add(0, new List<int>());
                this.Places.Add(1, new List<int>());
            }
            StringBuilder places = new StringBuilder();
            foreach (int place in this.Places[0])
            {
                places.Append(Pathfinding.GetCellChars(place));
            }
            places.Append("|");
            foreach (int place in this.Places[1])
            {
                places.Append(Pathfinding.GetCellChars(place));
            }
            this.Map.Places = places.ToString();
            this.Map.Save();
        }

        public void InitPlaces()
        {
            if (this.Map.Places != "")
            {
                this.Places.Add(0, new List<int>());
                this.Places.Add(1, new List<int>());
                //foreach (string place in this.Map.Places.Split(';').ToList())
                //{
                //    if (place != null)
                //    {
                //        if (place != "")
                //        {
                //            string[] data = place.Split(',');
                //            int teamID = int.Parse(data[1]);
                //            int cellID = int.Parse(data[0]);
                //            this.Places[teamID].Add(cellID);
                //        }
                //    }
                //}
                var p1 = this.Map.Places.Split('|')[0];
                var p2 = this.Map.Places.Split('|')[1];
                if (p1 != "")
                {
                    for (int i = 0; i < p1.Length; i += 2)
                    {
                        this.Places[0].Add(Pathfinding.GetCellNum(p1.Substring(i, 2)));
                    }
                }
                if (p2 != "")
                {
                    for (int i = 0; i < p2.Length; i += 2)
                    {
                        this.Places[1].Add(Pathfinding.GetCellNum(p2.Substring(i, 2)));
                    }
                }
            }
        }

        public void AddPlayer(World.Network.WorldClient client)
        {
            try
            {
                try
                {
                    UncompressMap(this.Map.MapData);
                    ShowFightCountOnMap(client);
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't add player on map : " + e.ToString());
                }
                Players.ShowPlayer(client);
                Players.CharactersOnMap.Add(client);
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't add player on map : " + e.ToString());
            }
        }

        public void AddNpc(Database.Records.NpcPositionRecord npc)
        {
            npc.TempID = _currentTempId;
            _currentTempId--;
            Npcs.Add(npc);
        }

        public void RemovePlayer(World.Network.WorldClient client)
        {
            try
            {
                Players.HidePlayer(client);
                Players.CharactersOnMap.Remove(client);
            }
            catch(Exception e){
                Utilities.ConsoleStyle.Error("Can't hide player : " + e.ToString());
            }
        }

        public void AddFightOnMap(World.Game.Fights.Fight fight)
        {
            this.ShowFight(fight);
            this.FightsOnMap.Add(fight);
            this.ShowFightCountOnMap();
        }

        public void RemoveFightOnMap(World.Game.Fights.Fight fight)
        {
            if (FightsOnMap.Contains(fight))
            {
                if (fight.State == World.Game.Fights.Fight.FightState.PlacementsPhase)
                {
                    this.HideFight(fight);
                }
                this.FightsOnMap.Remove(fight);
                this.ShowFightCountOnMap();
            }
        }

        public void RemoveMonstersOnMap(Engines.Map.MonsterGroup monsters)
        {
            this.Send("GM|-" + monsters.ID);
            this.Spawner.GroupsOnMap.Remove(monsters);
        }

        public void AddNewDroppedItem(Database.Records.WorldItemRecord item, int cellid)
        {
            this.ShowDroppedItem(item, cellid);
            this.DroppedItems.Add(cellid, item);
        }

        public void RemoveDroppedItem(Database.Records.WorldItemRecord item, int cellid)
        {
            this.UnShowDroppedItem(cellid);
            this.DroppedItems.Remove(cellid);
        }

        public void Send(string packet)
        {
            Players.CharactersOnMap.ForEach(x => x.Send(packet));
        }

        #endregion

        #region Getting

        public int GetActorAvailableID
        {
            get
            {
                _currentTempId--;
                return _currentTempId;
            }
        }

        public bool CharacterIsOnMap(string character)
        {
            if (Players.CharactersOnMap.FindAll(x => x.Character.Nickname == character).Count > 0) return true;
            return false;
        }

        public Database.Records.NpcPositionRecord GetNpc(int tempID)
        {
            return Npcs.First(x => x.TempID == tempID);
        }

        public Database.Records.NpcPositionRecord GetNpcByTemplate(int ID)
        {
            try
            {
                return Npcs.First(x => x.Template.ID == ID);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public World.Network.WorldClient GetClientOnMap(int id)
        {
            if (Players.CharactersOnMap.FindAll(x => x.Character.ID == id).Count > 0)
                return Players.CharactersOnMap.FirstOrDefault(x => x.Character.ID == id);
            return null;
        }

        public World.Game.Fights.Fight GetFight(int id)
        {
            if (this.FightsOnMap.FindAll(x => x.ID == id).Count > 0)
                return this.FightsOnMap.FirstOrDefault(x => x.ID == id);
            return null;
        }

        public Map.MonsterGroup GetMonsterGroupOnCell(int cellid)
        {
            if (this.Spawner.GroupsOnMap.FindAll(x => x.CellID == cellid).Count > 0)
                return this.Spawner.GroupsOnMap.FirstOrDefault(x => x.CellID == cellid);
            return null;
        }

        public Database.Records.ZaapRecord Zaap
        {
            get
            {
                return World.Helper.ZaapHelper.GetZaap(this.Map.ID);
            }
        }

        public Database.Records.WorldItemRecord GetDroppedItem(int cellid)
        {
            if (this.DroppedItems.ToList().FindAll(x => x.Key == cellid).Count > 0)
            {
                return this.DroppedItems.ToList().FirstOrDefault(x => x.Key == cellid).Value;
            }
            return null;
        }

        public Database.Records.PaddockRecord GetPaddock(int cellid)
        {
            if (this.Paddocks.FindAll(x => x.CellID == cellid).Count > 0)
            {
                return this.Paddocks.FirstOrDefault(x => x.CellID == cellid);
            }
            else
            {
                return null;
            }
        }

        public World.Game.IO.InteractiveObject GetIO(int cell)
        {
            if (this.InteractiveObjects.ContainsKey(cell))
            {
                return this.InteractiveObjects[cell];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Decompressor

        private static string ZKARRAY = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        public static int IndexArray(string a)
        {
            return ZKARRAY.IndexOf(a);
        }

        public List<Cell> FreeCells()
        {
            return this.Cells.Values.ToList().FindAll(x => x.Available != 0);
        }

        public Cell RandomFreeCell()
        {
            return this.FreeCells()[Utilities.Basic.Rand(0, this.FreeCells().Count)];
        }

        public bool IsAvailableCell(int cell)
        {
            return this.FreeCells().FindAll(x => x.ID == cell).Count > 0;
        }

        public Cell UncompressCell(string sData, int id)
        {
            Cell loc5 = new Cell(id, this.Map);
            string loc6 = sData;
            int loc7 = loc6.Length - 1;
            int[] loc8 = new int[8000];
            while (loc7 >= 0)
            {
                try
                {
                    loc8[loc7] = IndexArray(loc6[loc7].ToString());
                }
                catch (Exception e) { }
                loc7--;
            }
            try
            {
                loc5.Available = ((loc8[2] & 56) >> 3);
            }
            catch (Exception e)
            {
                loc5.Available = 1;
            }
            loc5.layerObject2Num = ((loc8[0] & 2) << 12) + ((loc8[7] & 1) << 12) + (loc8[8] << 6) + loc8[9];
            loc5.layerObject2Interactive = ((loc8[7] & 2) >> 1);
            loc5.layerObject1Num = ((loc8[0] & 4) << 11) + ((loc8[4] & 1) << 12) + (loc8[5] << 6) + loc8[6];
            return loc5;
        }

        public void UncompressMap(string data)
        {
            if (_cellsInit)
                return;
            try
            {
                this.mMatrix = new Cell[this.Map.Width, this.Map.Height];
                Dictionary<int, Cell> cells = new Dictionary<int, Cell>();
                int loc11 = data.Length;
                int loc13 = 0;
                int loc14 = 0;
                while (loc14 < loc11)
                {
                    try
                    {
                        Cell loc12 = UncompressCell(data.Substring(loc14, 10), loc13);
                        cells.Add(loc13, loc12);

                        if (loc12.layerObject2Num != 0 && loc12.layerObject2Interactive == 1)//If a interactive object
                        {
                            var ioObj = new World.Game.IO.InteractiveObject
                                (this.Map, (Enums.InteractiveObjectEnum)loc12.layerObject2Num, loc12.ID, World.Game.IO.InteractiveObjectState.FULL, true);
                            this.InteractiveObjects.Add(loc12.ID, ioObj);
                        }
                    }
                    catch (Exception e)
                    {
                        //Utilities.ConsoleStyle.Error("Can't decompress cell : " + e.ToString());
                    }
                    loc14 += 10;
                    loc13 += 1;
                }
                this.Cells = cells;
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant decompress map id '" + this.Map.ID + "' because : " + e.ToString());
            }
            finally
            {
                this._cellsInit = true;
            }
            try
            {
                this.Spawner.GenerateAllGroups();
            }catch(Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant init spawner in map id '" + this.Map.ID + "' because : " + e.ToString());
            }
            try
            {
                //this.SetupAStartPathfinding();
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant init pathfinding astar engine : " + e.ToString());
            }
        }

        private void SetupAStartPathfinding()
        {
            foreach (var cell in this.Cells)
            {
                Console.WriteLine(cell.Value.Coor.ToString());
                if (cell.Value.Coor.Y >= 0)
                {
                    this.mMatrix[cell.Value.Coor.Y, cell.Value.Coor.X] = cell.Value;
                }
            }
        }

        public bool IsFree(int cell)
        {
            return this.FreeCells().FindAll(x => x.ID == cell).Count > 0;
        }

        #endregion

    }
}
