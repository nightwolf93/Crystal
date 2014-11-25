using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Admin
{
    public class AdminRank
    {
        public int RankID { get; set; }
        public string Name { get; set; }
        public bool SuperAdmin { get; set; }
        public List<string> Permissions = new List<string>();

        public bool HasPermission(string perm)
        {
            if (this.SuperAdmin) return true;
            return this.Permissions.Contains(perm);
        }
    }
}
