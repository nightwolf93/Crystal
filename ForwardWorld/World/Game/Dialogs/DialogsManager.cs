using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Dialogs
{
    public static class DialogsManager
    {
        public static Dictionary<int, Interop.PythonScripting.PyScript> RegisteredScripts = new Dictionary<int, Interop.PythonScripting.PyScript>();

        public static bool ExistScript(int response)
        {
            lock (RegisteredScripts)
            {
                return RegisteredScripts.ContainsKey(response);
            }
        }
    }
}
