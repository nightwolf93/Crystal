using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Crystal.WorldServer.World.Game.Fights;

namespace Crystal.WorldServer.Interop.PythonScripting
{
    public class PyScript
    {
        public string Path { get; set; }
        public ScriptEngine Engine { get; set; }
        public ScriptScope Scope { get; set; }
        public PyScriptPlatform Platform { get; set; }
        public bool IsPlugin = false;

        public List<TimedEvent> Events = new List<TimedEvent>();

        public PyScript(string path)
        {
            this.Path = path;
            this.Platform = new PyScriptPlatform(this);
        }

        public void Load()
        {
            this.Engine = Python.CreateEngine();
            this.Scope = Engine.CreateScope();
            this.initScope();
            this.Engine.ExecuteFile(this.Path, this.Scope);
            this.DoMethod("init()");
        }

        public void Load(Fighter monster)
        {
            this.Engine = Python.CreateEngine();
            this.Scope = Engine.CreateScope();
            this.initScope();
            this.Engine.ExecuteFile(this.Path, this.Scope);
            this.Scope.SetVariable("monster", monster);
            this.DoMethod("init(monster)");
            this.Scope.RemoveVariable("monster");        
        } 

        private void initScope()
        {
            Scope.SetVariable("log", ScriptManager.Logger);
            Scope.SetVariable("API", this.Platform);
        }

        public void DoMethod(string method)
        {
            if (this.Engine != null)
            {
                try
                {
                    this.Engine.Execute(method, this.Scope);
                }
                catch (Exception e)
                {
                    if(this.Platform.showErrors)
                        Utilities.ConsoleStyle.Error("Error in method : " + e.ToString());
                }
            }
        }

        public dynamic DoMethodReturn(string method)
        {
            if (this.Engine != null)
            {
                try
                {
                    return this.Engine.Execute(method, this.Scope);
                }
                catch (Exception e)
                {
                    if (this.Platform.showErrors)
                        Utilities.ConsoleStyle.Error("Error in method : " + e.ToString());
                    return null;
                }
            }
            return null;
        }

        public void CallEventEnterMap(World.Network.WorldClient client, int mapid, int cellid)
        {
            try
            {
                if (!this.IsPlugin)
                {
                    this.Scope.SetVariable("player", client);
                    this.Scope.SetVariable("mapid", mapid);
                    this.Scope.SetVariable("cellid", cellid);

                    this.DoMethod("onEnterMap(player, mapid, cellid)");

                    this.Scope.RemoveVariable("player");
                    this.Scope.RemoveVariable("mapid");
                    this.Scope.RemoveVariable("cellid");
                }
            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)
                    Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
            }
        }

        public void CallEventNpcResponse(World.Network.WorldClient client, int response)
        {
            lock (this.Scope)
            {
                try
                {
                    if (!this.IsPlugin)
                    {
                        this.Scope.SetVariable("player", client);
                        this.Scope.SetVariable("response", response);

                        this.DoMethod("onPlayerNpcResponse(player, response)");

                        this.Scope.RemoveVariable("player");
                        this.Scope.RemoveVariable("response");
                    }
                }
                catch (Exception e)
                {
                    if (this.Platform.showErrors)
                        Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
                }
            }
        }

        public void CallEventPlayerSpeak(World.Network.WorldClient client, string message, string channel)
        {
            try
            {
                if (!this.IsPlugin)
                {
                    this.Scope.SetVariable("player", client);
                    this.Scope.SetVariable("message", message);
                    this.Scope.SetVariable("channel", channel);

                    this.DoMethod("onPlayerSpeak(player, message, channel)");

                    this.Scope.RemoveVariable("player");
                    this.Scope.RemoveVariable("message");
                    this.Scope.RemoveVariable("channel");
                }
            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)
                    Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
            }
        }

        public void CallEventPlayerMovement(World.Network.WorldClient client, int oldCell, int newCell)
        {
            try
            {
                if (!this.IsPlugin)
                {
                    this.Scope.SetVariable("player", client);
                    this.Scope.SetVariable("oldCell", oldCell);
                    this.Scope.SetVariable("newCell", newCell);

                    this.DoMethod("onPlayerMovement(player, oldCell, newCell)");

                    this.Scope.RemoveVariable("player");
                    this.Scope.RemoveVariable("oldCell");
                    this.Scope.RemoveVariable("newCell");
                }
            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)    
                    Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
            }
        }

        public void CallEventWinBattleVersusMonster(World.Network.WorldClient client, int mapid)
        {
            try
            {
                if (!this.IsPlugin)
                {
                    this.Scope.SetVariable("player", client);
                    this.Scope.SetVariable("mapid", mapid);

                    this.DoMethod("onWinBattleVersusMonster(player, mapid)");

                    this.Scope.RemoveVariable("player");
                    this.Scope.RemoveVariable("mapid");
                }
            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)
                    Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
            }
        }

        public void CallPerformAI(World.Game.Fights.Fighter fighter)
        {
            try
            {
                if (!this.IsPlugin)
                {
                    this.Scope.SetVariable("monster", fighter);

                    this.DoMethod("onPerformAI(monster)");

                    this.Scope.RemoveVariable("monster");
                }
            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)
                 Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
            }
        }

        public int CallTakingDamages(World.Game.Fights.Fighter fighter, int damages)
        {
            try
            {
                var returnedDamages = damages;

                this.Scope.SetVariable("monster", fighter);
                this.Scope.SetVariable("damages", damages);

                returnedDamages = this.DoMethodReturn("onTakingDamages(monster, damages)");

                this.Scope.RemoveVariable("monster");
                this.Scope.RemoveVariable("damages");

                return returnedDamages;

            }
            catch (Exception e)
            {
                if (this.Platform.showErrors)
                    Utilities.ConsoleStyle.Error("Can't call script : " + e.ToString());
                return damages;
            }
        }
    }

    public class TimedEvent
    {
        public string Method { get; set; }
        public int Time { get; set; }
        public Timer DelayerTimer { get; set; }
        public PyScript Script { get; set; }

        public TimedEvent(string method, int time, PyScript script)
        {
            this.Method = method;
            this.Time = time;
            this.Script = script;
            this.DelayerTimer = new Timer(time);
            this.DelayerTimer.Enabled = true;
            this.DelayerTimer.Elapsed += new ElapsedEventHandler(DelayerTimer_Elapsed);
            this.DelayerTimer.Start();
        }

        public void Destroy()
        {
            this.DelayerTimer.Enabled = false;
            this.DelayerTimer.Stop();
            this.DelayerTimer.Close();
        }

        private void DelayerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock(this.Script)
            {
                this.Script.Scope.SetVariable("timer", this);
                this.Script.DoMethod(Method + "(timer)");
                this.Script.Scope.RemoveVariable("timer");
            }
        }
    }
}
