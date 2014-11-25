using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class DialogHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("DR", typeof(DialogHandler).GetMethod("Respond"));
        }

        public static void Respond(World.Network.WorldClient client, string packet)
        {
            if (client.State == Network.WorldClientState.OnDialog)
            {
                string[] data = packet.Substring(2).Split('|');
                NpcHandler.ExitDialog(client);
                if (Game.Dialogs.DialogsManager.ExistScript(int.Parse(data[1])))
                {
                    Game.Dialogs.DialogsManager.RegisteredScripts[int.Parse(data[1])].CallEventNpcResponse(client, int.Parse(data[1]));
                }
                else
                {
                    Interop.Scripting.ScriptManager.CallScript("npc_response", data[1], client);
                }
            }    
        }
    }
}
