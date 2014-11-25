using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Crystal.WorldServer.World.Game.Idle
{
    public static class IdleManager
    {
        public static Timer IdleTimer { get; set; }

        public static void Start()
        {
            IdleTimer = new Timer(10000);
            IdleTimer.Enabled = true;
            IdleTimer.Elapsed += new ElapsedEventHandler(IdleTimer_Elapsed);
            IdleTimer.Start();
        }

        private static void IdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (IdleTimer)//Ou cas ou ?
            {
                foreach (var client in World.Helper.WorldHelper.GetClientsArray)
                {
                    if (client.Character != null)//On deconnect que ceux sont sur un personnage, on verras plus tard pour les autres
                    {
                        if (client.Character.Fighter == null)//On deconnecte que ceux qui sont dans le world et pas en combat
                        {
                            if (client.LastActionTime <= Environment.TickCount)
                            {
                                Utilities.ConsoleStyle.Infos("Disconnect player @'" + client.Character.Nickname + "'@ for inactivity");
                                client.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
