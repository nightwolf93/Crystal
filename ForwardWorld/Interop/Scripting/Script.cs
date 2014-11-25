using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Interop.Scripting
{
    public class Script
    {
        public string Name = "Untitled";
        public string Path = "?";

        public List<ScriptArgs> Args = new List<ScriptArgs>();

        public Script(string path)
        {
            this.Path = path;
            Read();
        }

        public void Read()
        {
            try
            {
                StreamReader reader = new StreamReader(Path);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Replace(" ", "");
                    if (line.StartsWith("~")) continue;
                    Args.Add(new ScriptArgs(line));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant load the script '" + e.ToString() + "' !");
            }
        }

        public ScriptArgs GetCallBy()
        {
            foreach (ScriptArgs arg in Args)
            {
                try
                {
                    if (arg.Args[0] == "callby")
                    {
                        return arg;
                    }
                }
                catch (Exception e)
                {

                }
            }
            return new ScriptArgs("null -> null");
        }

        public string GetName()
        {
            foreach (ScriptArgs arg in Args)
            {
                if (arg.Args.Count > 0)
                {
                    if (arg.Args[0] == "name")
                    {
                        return arg.Args[1];
                    }
                }
            }
            return null;
        }

        public ScriptArgs GetRef()
        {
            foreach (ScriptArgs arg in Args)
            {
                if (arg.Args.Count > 0)
                {
                    if (arg.Args[0] == "ref")
                    {
                        return arg;
                    }
                }
            }
            return null;
        }

        public void Execute(params object[] parameters)
        {
            try
            {
                switch (GetCallBy().Args[1])
                {
                    case "npc_response":
                        if (GetRef().GetIntValue(1) == int.Parse(parameters[0].ToString()))
                        {
                            World.Network.WorldClient client = (World.Network.WorldClient)parameters[1];
                            Args.ForEach(x => ExecuteWithClientArg(x, client));
                        }
                        break;

                    case "use_item":
                        if (GetRef().GetIntValue(1) == int.Parse(parameters[0].ToString()))
                        {
                            World.Network.WorldClient client = (World.Network.WorldClient)parameters[1];
                            Args.ForEach(x => ExecuteWithClientArg(x, client, parameters[2]));
                        }
                        break;

                    case "command":
                        if (GetRef().GetStringValue(1) == parameters[0].ToString())
                        {
                            World.Network.WorldClient client = (World.Network.WorldClient)parameters[1];
                            Args.ForEach(x => ExecuteWithClientArg(x, client, parameters[2]));
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Error when call cspt : " + e.ToString());
            }
        }

        public void ExecuteWithClientArg(ScriptArgs args, World.Network.WorldClient client, params object[] parameters)
        {
            try
            {
                switch (args.Args[0])
                {
                    case "player":
                        switch (args.Args[1])
                        {
                            case "goto":
                                World.Network.World.GoToMap(client, args.GetIntValue(2), args.GetIntValue(3));
                                break;

                            case "respawn":
                                World.Network.World.GoToMap(client, client.Character.SaveMap, client.Character.SaveCell);
                                break;

                            case "dialog":
                                switch (args.Args[2])
                                {
                                    case "start":
                                        //TODO!
                                        break;
                                }
                                break;

                            case "life":
                                switch (args.Args[2])
                                {
                                    case "restore":
                                        client.Action.Regen(int.Parse(args.Args[3]));
                                        break;
                                }
                                break;

                            case "message":
                                client.Action.SystemMessage(args.GetStringValue(2));
                                break;

                            case "align":
                                switch (args.Args[2])
                                {
                                    case "set":
                                        if (client.Character.Fighter == null)
                                        {
                                            client.Character.Faction.SetAlign(int.Parse(args.Args[3]));
                                        }
                                        else
                                        {
                                            client.Action.SystemMessage("Impossible en combat !");
                                        }
                                        break;
                                }
                                break;

                            case "need":
                                switch (args.Args[2])
                                {
                                    case "mj":
                                        if (client.Account.AdminLevel < int.Parse(args.Args[3]))
                                        {
                                            client.Action.SystemMessage("Vous ne posseder pas les conditions requises !");
                                            return;
                                        }
                                        break;
                                }
                                break;

                            case "look":
                                switch (args.Args[2])
                                {
                                    case "set":
                                        client.Character.Look = int.Parse(args.Args[3]);
                                        client.Action.RefreshRoleplayEntity();
                                        break;

                                    case "normal":
                                        
                                        break;
                                }
                                break;

                            case "scale":
                                switch (args.Args[2])
                                {
                                    case "set":
                                        client.Character.Scal = int.Parse(args.Args[3]);
                                        client.Action.RefreshRoleplayEntity();
                                        break;

                                    case "normal":
                                        client.Character.Scal = 100;
                                        client.Action.RefreshRoleplayEntity();
                                        break;
                                }
                                break;

                            case "elite":
                                switch (args.Args[2])
                                {
                                    case "up":
                                        World.Game.Elite.EliteManager.UpElite(client);
                                        break;
                                }
                                break;

                            case "level":
                                switch (args.Args[2])
                                {
                                    case "set":
                                        if (client.Character.Fighter == null)
                                        {
                                            Database.Records.ExpFloorRecord floor = World.Helper.ExpFloorHelper.GetCharactersLevelFloor(int.Parse(args.Args[3]));
                                            client.Character.Experience = floor.Character;
                                            client.Action.TryLevelUp();
                                        }
                                        else
                                        {
                                            client.Action.SystemMessage("Impossible en combat !");
                                        }
                                        break;

                                    case "add":
                                        if (client.Character.Fighter == null)
                                        {
                                            int addedLevel = int.Parse(args.Args[3]);
                                            int nextLevel = client.Character.Level + addedLevel;
                                            if (nextLevel > 5000)
                                            {
                                                nextLevel = 5000;
                                            }
                                            Database.Records.ExpFloorRecord floor = World.Helper.ExpFloorHelper.GetCharactersLevelFloor(nextLevel);
                                            client.Character.Experience = floor.Character;
                                            client.Action.TryLevelUp();
                                        }
                                        else
                                        {
                                            client.Action.SystemMessage("Impossible en combat !");
                                        }
                                        break;
                                }
                                break;
                        }
                        break;

                    case "ui":
                        switch (args.Args[1])
                        {
                            case "show":
                                switch (args.Args[2])
                                {
                                    case "paddock":
                                        client.Character.Map.Engine.ShowPaddocksMounts(client);
                                        break;

                                    case "guild":
                                        client.Send("gn");
                                        break;
                                }
                                break;
                        }
                        break;

                    case "this":
                        switch (args.Args[1])
                        {
                            case "item":
                                switch (args.Args[2])
                                {
                                    case "remove":
                                        Database.Records.WorldItemRecord toDeleteItem = (Database.Records.WorldItemRecord)parameters[0];
                                        client.Character.Items.RemoveItem(toDeleteItem, 1);
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }
            catch { }     
        }
    }
}
