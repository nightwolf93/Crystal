using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;

namespace Crystal.WorldServer.Interop.Plugins.InteropClass
{
    
    public class OutputLog
    {
        public string User { get; set; }
        public ConsoleColor Color { get; set; }

        public OutputLog(string user, ConsoleColor color)
        {
            this.User = user;
            this.Color = color;
        }

        public void Print(string message)
        {
            Utilities.ConsoleStyle.Append("[" + this.User + "]", message, this.Color);
        }
    }
}
