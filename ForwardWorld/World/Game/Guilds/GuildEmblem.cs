using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Guilds
{
    public class GuildEmblem
    {
        public int BackID { get; set; }
        public int BackColor { get; set; }
        public int FrontID { get; set; }
        public int FrontColor { get; set; }

        public GuildEmblem(int backID, int backColor, int frontID, int frontColor)
        {
            this.BackID = backID;
            this.BackColor = backColor;
            this.FrontID = frontID;
            this.FrontColor = frontColor;
        }

        public override string ToString()
        {
            return Utilities.Base36.Encode(this.BackID) + "," + Utilities.Base36.Encode(this.BackColor) + 
                "," + Utilities.Base36.Encode(this.FrontID) + "," + Utilities.Base36.Encode(this.FrontColor);
        }
    }
}
