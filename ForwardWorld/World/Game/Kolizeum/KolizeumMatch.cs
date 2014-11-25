using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Kolizeum
{
    public class KolizeumMatch
    {
        public KolizeumTeam RedTeam { get; set; }
        public KolizeumTeam BlueTeam { get; set; }

        public Game.Fights.Fight Fight { get; set; }

        public bool Started = false;

        public KolizeumMap Map { get; set; }

        public KolizeumMatch(KolizeumTeam team1, KolizeumTeam team2)
        {
            this.RedTeam = team1;
            this.BlueTeam = team2;
        }

        /// <summary>
        /// Do teleportation for the two teams
        /// </summary>
        public void InitializeMatch()
        {
            if (!this.Started)
            {
                this.teleportTeam(this.RedTeam);
                this.teleportTeam(this.BlueTeam);

                //Need sleep for the client latence --"
                System.Threading.Thread.Sleep(3500);

                this.Fight = new Fights.Fight(new Game.Fights.Fighter(this.RedTeam.FirstFighter)
                    , new Game.Fights.Fighter(this.BlueTeam.FirstFighter), this.Map.Map.Engine, Enums.FightTypeEnum.Kolizeum);
                
                /* Destroy on the map */
                this.BlueTeam.FirstFighter.Character.Map.Engine.RemovePlayer(this.BlueTeam.FirstFighter);
                this.RedTeam.FirstFighter.Character.Map.Engine.RemovePlayer(this.RedTeam.FirstFighter);

                /* Enable fight context */
                System.Threading.Thread.Sleep(1500);
                this.Fight.EnableContext();
                foreach (var fighter in this.BlueTeam.GetClientsExceptFirst)
                {
                    System.Threading.Thread.Sleep(1500);
                    fighter.Character.Map.Engine.RemovePlayer(fighter);
                    this.Fight.AddPlayer(new Fights.Fighter(fighter), 0);
                }
                System.Threading.Thread.Sleep(4000);
                foreach (var fighter in this.RedTeam.GetClientsExceptFirst)
                {
                    System.Threading.Thread.Sleep(1500);
                    fighter.Character.Map.Engine.RemovePlayer(fighter);
                    this.Fight.AddPlayer(new Fights.Fighter(fighter), 1);
                }
                System.Threading.Thread.Sleep(2000);
                this.Fight.TryStartFight(true);

                this.Message("Votre combat a debuter bonne chance a vous !<br /><b>Recompense de ce combat</b> : Aucune pour la beta");

                this.Started = true;
            }
        }

        public List<Network.WorldClient> Fighters
        {
            get
            {
                var fighters = new List<Network.WorldClient>();
                fighters.AddRange(this.RedTeam.Clients);
                fighters.AddRange(this.BlueTeam.Clients);
                return fighters;
            }
        }

        public void Message(string message)
        {
            this.Fighters.ForEach(x => x.Action.KolizeumMessage(message));
        }

        private void teleportTeam(KolizeumTeam team)
        {
            foreach (var client in team.Clients)
            {
                client.Action.Regen(0, true);
                client.Action.KolizeumLastMapID = client.Character.MapID;
                client.Action.KolizeumLastCellID = client.Character.CellID;
                World.Network.World.GoToMap(client, this.Map.MapID, this.Map.CellID);
                System.Threading.Thread.Sleep(350);
            }
        }
    }
}
