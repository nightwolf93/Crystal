using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Crystal.WorldServer.World.Game.LiveActions
{
    /// <summary>
    /// LiveAction by NightWolf
    /// </summary>
    public static class LiveActionManager
    {
        public const int ITEM_ACTION_ID = 21;

        public static Timer LiveActionTimer { get; set; }

        public static void Initialize()
        {
            LiveActionTimer = new Timer(10000);
            LiveActionTimer.Elapsed += LiveActionTimer_Elapsed;
            LiveActionTimer.Enabled = true;
            LiveActionTimer.Start();
        }

        public static void ExecuteLiveAction()
        {
            var actions = Database.Records.LiveActionRecord.FindAll();
            foreach (var a in actions)
            {
                var client = World.Helper.WorldHelper.GetClientByCharacter(a.PlayerID);
                if (client != null)
                {
                    switch (a.Action)
                    {
                        case ITEM_ACTION_ID:
                            Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, a.Nombre);
                            client.Character.AddItem(item, 1);
                            client.Action.SystemMessage("L'objet <b>" + item.GetTemplate.Name + "</b> a ete ajouter a votre inventaire, merci de votre achat !");
                            client.Action.SaveContents();
                            break;

                        default:
                            Utilities.ConsoleStyle.Error("Action ID no supported yet !");
                            break;
                    }

                    //Delete the record
                    a.Delete();
                }
            }
        }

        private static void LiveActionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ExecuteLiveAction();   
        }
    }
}
