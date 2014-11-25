using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Commands
{
    public static class CommandsManager
    {
        public static Dictionary<string, AbstactCommand> CommandsRegistered = new Dictionary<string, AbstactCommand>();

        public static void RegisterCommand(string header, string description, int level)
        {

        }

        public static bool ExistCommand(string header)
        {
            lock (CommandsRegistered)
            {
                return CommandsRegistered.ContainsKey(header);
            }
        }
    }
}
