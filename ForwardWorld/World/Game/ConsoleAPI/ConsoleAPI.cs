using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Pathfinder;

namespace Crystal.WorldServer.World.Game.ConsoleAPI
{
    public class ConsoleAPI
    {
        public World.Network.WorldClient self = null;

        public ConsoleAPI(World.Network.WorldClient client)
        {
            this.self = client;
        }

        public static void Execute(World.Network.WorldClient client, string command)
        {
            var console = new ConsoleAPI(client);
            var api = Interop.PythonScripting.ScriptManager.GetConsoleAPI();
            api.Scope.SetVariable("self", client);
            api.Scope.SetVariable("console", console);

            api.DoMethod(command);

            api.Scope.RemoveVariable("console");
            api.Scope.RemoveVariable("self");
        }

        public void TestPath(int startCell, int endCell)
        {
            this.self.APIShowCell(startCell); this.self.APIShowCell(endCell);

            var finder = new PathfindingV2(this.self.APIGetMap().Engine);
            var result = finder.FindShortestPath(startCell, endCell, new List<int>());
            this.self.APIMessage("Taille : " + result.Count);
            result.ForEach(x => this.self.APIShowCell(x.ID));
        }

        public void log(string message)
        {
            this.self.Action.SystemMessage(message);
        }

        public void logColor(string message, string color)
        {
            this.self.Action.BasicMessage(message, color);
        }

        public void reload()
        {
            Interop.PythonScripting.ScriptManager.Load();
        }
    }
}
