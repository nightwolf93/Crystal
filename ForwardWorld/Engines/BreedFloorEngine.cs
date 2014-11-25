using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines
{
    public class BreedFloorEngine
    {
        public Database.Records.BreedRecord Breed { get; set; }

        public List<Breeds.StatFloor> LifeFloors = new List<Breeds.StatFloor>();
        public List<Breeds.StatFloor> WisdomFloors = new List<Breeds.StatFloor>();
        public List<Breeds.StatFloor> StrenghtFloors = new List<Breeds.StatFloor>();
        public List<Breeds.StatFloor> FireFloors = new List<Breeds.StatFloor>();
        public List<Breeds.StatFloor> LuckFloors = new List<Breeds.StatFloor>();
        public List<Breeds.StatFloor> AgilityFloors = new List<Breeds.StatFloor>();

        public BreedFloorEngine(Database.Records.BreedRecord breed)
        {
            this.Breed = breed;
            this.Load();
        }

        public void Load()
        {
            this.loadStatsFloor(this.LifeFloors, this.Breed.LifeFloor);
            this.loadStatsFloor(this.WisdomFloors, this.Breed.WisdomFloor);
            this.loadStatsFloor(this.StrenghtFloors, this.Breed.StrenghtFloor);
            this.loadStatsFloor(this.FireFloors, this.Breed.FireFloor);
            this.loadStatsFloor(this.LuckFloors, this.Breed.LuckFloor);
            this.loadStatsFloor(this.AgilityFloors, this.Breed.AgilityFloor);
        }

        private void loadStatsFloor(List<Breeds.StatFloor> floors, string data)
        {
            foreach (string floor in data.Split('|'))
            {
                if (floor != "")//Check if nothing
                {
                    string[] basicFloorInfos = floor.Split(':');
                    string[] intervall = basicFloorInfos[0].Split(',');
                    string[] amounts = basicFloorInfos[1].Split('-');
                    if (intervall.Length > 1)
                    {
                        floors.Add(new Breeds.StatFloor(int.Parse(intervall[0]), int.Parse(intervall[1]), int.Parse(amounts[0]), int.Parse(amounts[1])));
                    }
                    else
                    {
                        floors.Add(new Breeds.StatFloor(int.Parse(intervall[0]), int.MaxValue, int.Parse(amounts[0]), int.Parse(amounts[1])));
                    }
                }
            }
        }

        private Breeds.StatFloor getFloor(List<Breeds.StatFloor> floors, int value)
        {
            return floors.FindAll(x => x.From <= value).LastOrDefault(); 
        }

        public Breeds.StatFloor GetFloorForValue(Enums.StatsTypeEnum element, int floor)
        {
            switch (element)
            {
                case Enums.StatsTypeEnum.Life: return getFloor(this.LifeFloors, floor);
                case Enums.StatsTypeEnum.Wisdom: return getFloor(this.WisdomFloors, floor);
                case Enums.StatsTypeEnum.Strenght: return getFloor(this.StrenghtFloors, floor);
                case Enums.StatsTypeEnum.Fire: return getFloor(this.FireFloors, floor);
                case Enums.StatsTypeEnum.Water: return getFloor(this.LuckFloors, floor);
                case Enums.StatsTypeEnum.Agility: return getFloor(this.AgilityFloors, floor);
                default: return null;
            }
        }
    }
}
