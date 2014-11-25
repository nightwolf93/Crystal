using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Tickets
{
    public static class TicketsManager
    {
        public static Dictionary<string, string> Tickets = new Dictionary<string, string>();

        public static void WantAddTicket(World.Network.WorldClient client, string message)
        {
            if (!Tickets.ContainsKey(client.Character.Nickname))
            {
                Tickets.Add(client.Character.Nickname, message);
                client.Action.SystemMessage("Merci, votre ticket seras traiter prochainement par notre equipe, faite <b>(.ticket close)</b> si vous avez resolus votre probleme sans notre aide, bon jeu sur Arkalia !");
            }
            else
            {
                client.Action.SystemMessage("Votre ticket n'a pas encore ete traiter veuilliez patienter !");
            }
        }

        public static void CloseTicket(World.Network.WorldClient client)
        {
            if (Tickets.ContainsKey(client.Character.Nickname))
            {
                Tickets.Remove(client.Character.Nickname);
                client.Action.SystemMessage("Votre ticket a ete supprimer !");
            }
            else
            {
                client.Action.SystemMessage("Vous n'avez aucun ticket en attente !");
            }
        }
    }
}
