using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Crystal.WorldServer.World.Game.Pools
{
    public static class PoolManager
    {
        public static Timer WarnerTimer { get; set; }
        public static bool Finished = true;

        public static void StartPoolsWarning()
        {
            WarnerTimer = new Timer(Utilities.ConfigurationManager.GetIntValue("PoolWarningIntervall"));
            WarnerTimer.Enabled = true;
            WarnerTimer.Elapsed += new ElapsedEventHandler(WarnerTimer_Elapsed);
            WarnerTimer.Start();
        }

        public static void WarnerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Finished)
            {
                Finished = false;
                try
                {
                    foreach (var client in World.Helper.WorldHelper.GetClientsArray)
                    {
                        try
                        {
                            if (Arkalia.ArkaliaAPI.CanVote(client.Account.Username))
                            {
                                client.Send("M1MESSAGE_BIENVENUE!&#13         Vous pouvez desormais voter sur le launcher pour gagner 100 points boutique ! Nous vous le rappelerons a chaque fois que vous pourrez voter !");
                                client.Action.SystemMessage("<font color=\"#FF0000\"><b>Rappel :</b> Vous pouvez desormais voter pour gagner 100 points boutique ! Nous vous le rappelerons a chaque fois que vous pourrez voter !</font>");
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.ConsoleStyle.Error("Can't send pools warning : " + ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.ConsoleStyle.Error("Can't send pools warning : " + ex.ToString());
                }
                Finished = true;
            }
        }
    }
}
