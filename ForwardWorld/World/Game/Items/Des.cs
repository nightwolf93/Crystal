using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers.Items
{
    public class Des
    {
        public int Min
        {
            get;
            set;
        }

        public int Max
        {
            get;
            set;
        }

        public int Fix
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Min + "d" + Max + "+" + Fix;
        }

        public static int RandomDesJet(string des)
        {
            string[] data = des.Split('d');
            int value1 = int.Parse(data[0]);
            int value2 = int.Parse(data[1].Split('+')[0]);
            int value3 = int.Parse(data[1].Split('+')[0]);

            return Utilities.Basic.Rand(value1 + value3, value2 + value3);
        }
    }
}
