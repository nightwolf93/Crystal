using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Commands
{
    public class ScriptCommand : AbstactCommand
    {
        public Interop.PythonScripting.PyScript Script { get; set; }

        public ScriptCommand(Interop.PythonScripting.PyScript script, string header, string description, int level)
            : base(header, description, level)
        {
            this.Script = script;
        }

        public override void OnCall(World.Network.WorldClient client, string[] args)
        {
            lock (this.Script)
            {
                Script.Scope.SetVariable("player", client);
                Script.Scope.SetVariable("header", this.Header);
                Script.Scope.SetVariable("params", args);

                Script.DoMethod("onCommand(player, header, params)");

                Script.Scope.RemoveVariable("player");
                Script.Scope.RemoveVariable("header");
                Script.Scope.RemoveVariable("params");
            }
        }
    }
}
