using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Crystal.WorldServer.Interop.PythonScripting
{
    public class PyScriptPlatform
    {
        public PyScript Script { get; set; }
        public bool showErrors = true;

        public static Cache.PersistCache WorldMemory { get; set; }

        public static void LoadWorldMemory()
        {
            WorldMemory = new Cache.PersistCache("Datas/WorldMemory.cache");
        }

        public PyScriptPlatform(PyScript script)
        {
            this.Script = script;
        }

        public void registerCommand(string header, string description, int level)
        {
            World.Game.Commands.CommandsManager.CommandsRegistered.Add(header, 
                new World.Game.Commands.ScriptCommand(this.Script, header, description, level));
        }

        public void registerResponse(int responseID)
        {
            if (World.Game.Dialogs.DialogsManager.RegisteredScripts.ContainsKey(responseID))
                World.Game.Dialogs.DialogsManager.RegisteredScripts.Remove(responseID);

            World.Game.Dialogs.DialogsManager.RegisteredScripts.Add(responseID, this.Script);
        }

        public void registerTimedEvent(string method, int time)
        {
            var e = new TimedEvent(method, time, this.Script);
            this.Script.Events.Add(e);
        }

        public Database.Records.MapRecords FindMap(int id)
        {
            return World.Helper.MapHelper.FindMap(id);
        }

        public void Sleep(int time)
        {
            System.Threading.Thread.Sleep(time);
        }

        public int Rand(int min, int max)
        {
            return Utilities.Basic.Rand(min, max);
        }

        public void WorldMessage(string message, string color = "#FF0000")
        {
            World.Manager.WorldManager.SendMessage(message, color);
        }

        public void UIOpenPaddock(World.Network.WorldClient client)
        {
            client.Character.Map.Engine.ShowPaddocksMounts(client);
        }

        public Cache.PersistCache GetWorldMemory()
        {
            return WorldMemory;
        }

        public string WebGet(string url)
        {
            var data = "";
            var web = new WebClient();
            data = web.DownloadString(url);
            web.Dispose();
            return data;
        }

        public void Log(string message)
        {
            Utilities.ConsoleStyle.Script(message, this.Script.Path);
        }

        public void LoadScript(string script)
        {
            this.Script.Engine.ExecuteFile(script, this.Script.Scope);
        }

        public void IsPlugin()
        {
            this.Script.IsPlugin = true;
        }
    }
}
