using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crystal.WorldServer.Interop
{
    public class StartArgsManager
    {
        public static void ProcessArgs(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var p in args)
                {
                    if (p != "")
                    {
                        if (p == "-gui")
                        {
                            Graphics.GraphicManager.HideConsole();
                            Application.Run(Program.AlternativeGui);
                        }
                    }
                }
            }
        }
    }
}
