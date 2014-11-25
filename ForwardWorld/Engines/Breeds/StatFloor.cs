using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Engines.Breeds
{
    public class StatFloor
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Value { get; set; }
        public int Cost { get; set; }

        public StatFloor(int from, int to, int value, int cost)
        {
            this.From = from;
            this.To = to;
            this.Value = value;
            this.Cost = cost;
        }
    }
}
