using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Commands
{
    public abstract class AbstactCommand
    {
        public string Header { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }

        public AbstactCommand(string header, string description, int level)
        {
            this.Header = header;
            this.Description = description;
            this.Level = level;
        }

        public virtual void OnCall(World.Network.WorldClient client, string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
