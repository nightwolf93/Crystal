using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines.Map
{
    public class MonsterGroup
    {
        public int ID;
        public Database.Records.MonsterLevelRecord Leader { get; set; }
        public int CellID;
        public int Dir = 3;
        public int Bonus = 0;
        public System.Timers.Timer BonusTimer { get; set; }

        public const int BONUS_TIME_RATE = 432000000;//432000000 = 2h

        public string CatchedPattern = ("");

        public List<Database.Records.MonsterLevelRecord> Monsters = 
                            new List<Database.Records.MonsterLevelRecord>();

        public List<Database.Records.WorldItemRecord> HardcoreItemsEarned = 
                            new List<Database.Records.WorldItemRecord>();

        public void AddMonster(Database.Records.MonsterLevelRecord monster)
        {
            if (Leader == null)
            {
                this.Leader = monster;
                this.BonusTimer = new System.Timers.Timer(BONUS_TIME_RATE);
                this.BonusTimer.Enabled = true;
                this.BonusTimer.Elapsed += new System.Timers.ElapsedEventHandler(BonusTimer_Elapsed);
                this.BonusTimer.Start();
            }
            Monsters.Add(monster);
            //Monsters.OrderBy(x => x.Level);
        }

        private void BonusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.Bonus >= 200)
            {
                this.StopBonusTimer();//Stop the calcul
                return;
            }

            //Adding new star :)
            this.Bonus = this.Bonus + 25;
            CreatePattern();
        }

        public void StopBonusTimer()
        {
            try
            {
                if (this.BonusTimer.Enabled)
                {
                    this.BonusTimer.Enabled = false;
                    this.BonusTimer.Stop();
                    this.BonusTimer.Close();
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't stop bonus timer : " + e.ToString());
            }
        }

        public void CreatePattern()
        {
            string gmInfo = "|+" + this.CellID + ";" + this.Dir + ";" + this.Bonus.ToString() + ";" + this.ID + ";";
            string monsterIDs = ("");
            string monstersSkins = ("");
            string monsterLevels = ("");
            string colors = ("");

            bool first = true;

            foreach (Database.Records.MonsterLevelRecord monster in this.Monsters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    monsterIDs += ",";
                    monstersSkins += ",";
                    monsterLevels += ",";
                    colors += ";";
                }
                Database.Records.MonstersTemplateRecord template = monster.GetTemplate;
                monsterIDs += template.ID.ToString();
                monstersSkins += template.Skin + "^" + monster.Size;
                monsterLevels += monster.Level.ToString();
                colors += template.Color1.ToString("x") + "," + template.Color2.ToString("x") + "," + template.Color3.ToString("x") + ";";
            }
            gmInfo += monsterIDs + ";-3;" + monstersSkins + ";" + monsterLevels + ";" + colors;
            this.CatchedPattern = gmInfo;
        }

        public override string ToString()
        {
            string strGrp = "{";
            strGrp += string.Join(",", Monsters);
            return strGrp + "}";
        }
    }
}
