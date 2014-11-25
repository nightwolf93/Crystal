using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("jobs_data")]
    public class JobDataRecord : ActiveRecordBase<JobDataRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID { get; set; }

        [Property("tools")]
        public string ToolsList { get; set; }

        [Property("crafts")]
        public string CraftList { get; set; }

        public Dictionary<int, List<int>> Crafts = new Dictionary<int, List<int>>();

        public void Initialize()
        {
            foreach (var c in this.CraftList.Split('|'))
            {
                if (c != "")
                {
                    var data = c.Split(';');
                    var skill = int.Parse(data[0]);
                    this.Crafts.Add(skill, new List<int>());
                    foreach (var t in data[1].Split(','))
                    {
                        if (t != "")
                        {
                            this.Crafts[skill].Add(int.Parse(t));
                        }
                    }
                }
            }
        }
    }
}
