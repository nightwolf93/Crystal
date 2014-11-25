using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.World.Network;

namespace Crystal.WorldServer.World.Handlers
{
    public static class FightHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("GQ", typeof(FightHandler).GetMethod("GameFightQuit"));
            Network.Dispatcher.RegisteredMethods.Add("Gp", typeof(FightHandler).GetMethod("GameFightPosition"));
            Network.Dispatcher.RegisteredMethods.Add("GR", typeof(FightHandler).GetMethod("GameReadyRequest"));
            Network.Dispatcher.RegisteredMethods.Add("Gt", typeof(FightHandler).GetMethod("GameTurnFinishRequest"));
            Network.Dispatcher.RegisteredMethods.Add("fS", typeof(FightHandler).GetMethod("GameFightBlockSpectatorRequest"));
            Network.Dispatcher.RegisteredMethods.Add("fN", typeof(FightHandler).GetMethod("GameFightFullBlock"));
            Network.Dispatcher.RegisteredMethods.Add("Gf", typeof(FightHandler).GetMethod("GameRequestShowCell"));
        }

        public static void GameFightQuit(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                if (packet.Length > 2)
                {
                    int id = int.Parse(packet.Substring(2));
                    var kickedFighter = client.Character.Fighter.Team.Fighters.FirstOrDefault(x => x.ID == id);
                    if (kickedFighter != null && client.Character.Fighter.Team.Leader.ID == client.Character.ID)
                    {
                        client.Character.Fighter.Team.Fight.QuitBattle(kickedFighter);
                    }
                }
                else
                {
                    client.Character.Fighter.Team.Fight.QuitBattle(client.Character.Fighter, true);
                }
            }
            else if(client.Action.Spectator != null)
            {
                client.Action.Spectator.WatchedFight.QuitSpectator(client.Action.Spectator);
                client.Action.Spectator = null;
            }
        }

        public static void GameFightPosition(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                if (client.Character.Fighter.Team.Fight.State == Game.Fights.Fight.FightState.PlacementsPhase)
                {
                    client.Character.Fighter.Team.Fight.ChangePlayerPlace(client.Character.Fighter, int.Parse(packet.Substring(2)));
                }
            }
        }

        public static void GameReadyRequest(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                client.Character.Fighter.Team.Fight.ChangeReadyState(client.Character.Fighter, int.Parse(packet.Substring(2)));
            }
        }

        public static void GameTurnFinishRequest(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                client.Character.Fighter.Team.Fight.FinishTurnRequest(client.Character.Fighter);
            }
        }

        public static void GameFightBlockSpectatorRequest(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                if (client.Character.Fighter.Team.Fight.BlockSpectator)
                {
                    client.Character.Fighter.Team.Fight.Send("Im039");
                    client.Character.Fighter.Team.Fight.BlockSpectator = false;
                }
                else
                {
                    client.Character.Fighter.Team.Fight.Send("Im040");
                    client.Character.Fighter.Team.Fight.BlockSpectator = true;
                }
            }
        }

        public static void GameFightFullBlock(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                if (client.Character.Fighter.Team.Fight.State == Game.Fights.Fight.FightState.PlacementsPhase)
                {
                    if (client.Character.Fighter.Team.Restrictions.FullBlocked)
                    {
                        client.Character.Fighter.Team.Restrictions.FullBlocked = false;
                        client.Character.Fighter.Team.Fight.Send("Im096;");
                        client.Character.Fighter.Team.Fight.Map.Send("Go-A" + client.Character.ID);
                    }
                    else
                    {
                        client.Character.Fighter.Team.Restrictions.FullBlocked = true;
                        client.Character.Fighter.Team.Fight.Send("Im095;");
                        client.Character.Fighter.Team.Fight.Map.Send("Go+A" + client.Character.ID);
                    }
                }
            }
        }

        public static void GameFightGroupBlock(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                if (client.Character.Fighter.Team.Fight.State == Game.Fights.Fight.FightState.PlacementsPhase)
                {
                    if (client.Character.Fighter.Team.Restrictions.OnlyParty)
                    {
                        client.Character.Fighter.Team.Restrictions.OnlyParty = false;
                        client.Character.Fighter.Team.Fight.Send("Im096;");

                    }
                    else
                    {
                        client.Character.Fighter.Team.Restrictions.OnlyParty = true;
                        client.Character.Fighter.Team.Fight.Send("Im095;");

                    }
                }
            }
        }

        public static void GameRequestShowCell(WorldClient client, string packet)
        {
            if (client.Character.Fighter != null)
            {
                var cellID = int.Parse(packet.Substring(2));
                client.Character.Fighter.Team.Send("Gf" + client.Character.ID + "|" + cellID);
            }
        }
    }
}
