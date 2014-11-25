using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Interop.Scripting
{
    public static class ScriptManager
    {
        public static List<Script> Scripts = new List<Script>();

        public static void Load(string path)
        {
            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                var f = new System.IO.FileInfo(file);
                if (f.Extension == ".txt")
                {
                    Scripts.Add(new Script(file));
                }
            }
            System.IO.Directory.GetDirectories(path).ToList().ForEach(x => Load(x));
        }

        public static List<Script> GetScriptsByCallType(string callType)
        {
            return Scripts.FindAll(x => x.GetCallBy().Args[1] == callType);
        }

        public static void CallScript(string callby, params object[] parameters)
        {
            foreach (Script script in GetScriptsByCallType(callby))
            {
                script.Execute(parameters);
            }
        }
    }
}
