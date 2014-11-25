using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Interop.Scripting
{
    public class ScriptArgs
    {
        public List<string> Args = new List<string>();

        public ScriptArgs(string args)
        {
            char[] separator = new char[2] { '-', '>', };
            string[] lineData = args.ToLower().Split(separator);
            foreach (string str in lineData)
            {
                if (str != "" && str != null)
                {
                    Args.Add(str);
                }
            }
        }

        public string GetStringValue(int index)
        {
            return Args[index];
        }

        public int GetIntValue(int index)
        {
            return int.Parse(Args[index]);
        }

        public bool GetBoolValue(int index)
        {
            return bool.Parse(Args[index]);
        }
    }
}
