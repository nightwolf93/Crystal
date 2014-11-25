using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.WorldServer.Interop.PythonScripting
{
    public static class ScriptManager
    {
        public static List<PyScript> Scripts = new List<PyScript>();

        public static Plugins.InteropClass.OutputLog Logger = new Plugins.InteropClass.OutputLog("PyScripts", ConsoleColor.DarkYellow);

        public static void Load()
        {
            PyScriptPlatform.LoadWorldMemory();
            Scripts.Clear();
            foreach (var f in Directory.GetFiles("Scripts"))
            {
                var fInfos = new FileInfo(f);
                if (fInfos.Extension.ToLower() == ".py")
                {
                    try
                    {
                        var script = new PyScript(f);
                        script.Load();
                        Scripts.Add(script);
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Can't load script engine for '" + f + "' : " + e.ToString());
                    }
                }
            }

            foreach (var f in Directory.GetFiles("Scripts/System"))
            {
                var fInfos = new FileInfo(f);
                if (fInfos.Extension.ToLower() == ".py")
                {
                    try
                    {
                        var script = new PyScript(f);
                        script.Load();
                        Scripts.Add(script);
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Can't load script engine for '" + f + "' : " + e.ToString());
                    }
                }
            }
        }

        public static PyScript GetConsoleAPI()
        {
            return Scripts.FirstOrDefault(x => x.Path.Contains("console_api.py"));
        }

        public static void CallEventEnterMap(World.Network.WorldClient client, int mapid, int cellid)
        {

            foreach (var script in Scripts)
            {
                script.CallEventEnterMap(client, mapid, cellid);
            }

        }

        public static void CallEventPlayerSpeak(World.Network.WorldClient client, string message, string channel)
        {

            foreach (var script in Scripts)
            {
                script.CallEventPlayerSpeak(client, message, channel);
            }

        }

        public static void CallEventPlayerMovement(World.Network.WorldClient client, int oldCell, int newCell)
        {

            foreach (var script in Scripts)
            {
                script.CallEventPlayerMovement(client, oldCell, newCell);
            }

        }

        public static void CallEventWinBattleVersusMonster(World.Network.WorldClient client, int mapid)
        {

            foreach (var script in Scripts)
            {
                script.CallEventWinBattleVersusMonster(client, mapid);
            }

        }
    }
}
