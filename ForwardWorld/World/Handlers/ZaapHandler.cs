using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class ZaapHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("WV", typeof(ZaapHandler).GetMethod("CloseZaapPanel"));
            Network.Dispatcher.RegisteredMethods.Add("WU", typeof(ZaapHandler).GetMethod("UseZaap"));
        }

        public static void OnRequestUse(World.Network.WorldClient client, int cell)
        {
            if (cell == client.Character.CellID)
            {
                OpenZaapPanel(client);
            }
            else
            {
                client.State = Network.WorldClientState.OnRequestZaap;
            }
        }

        public static void OpenZaapPanel(World.Network.WorldClient client)
        {
            StringBuilder packet = new StringBuilder("WC" + client.Character.SaveMap + "|");
            client.Character.Zaaps.ForEach(x => packet.Append(x + ";" + GetPriceOfTravel(client.Character.MapID, x) + "|"));
            client.Send(packet.ToString().Substring(0, packet.ToString().Length - 1));
        }

        public static int GetPriceOfTravel(int pos1, int pos2)
        {
            if (pos1 >= pos2)
            {
                return (pos1 - pos2) / 5;
            }
            else if (pos1 == pos2)
            {
                return 0;
            }
            else
            {
                return (pos2 - pos1) / 5;
            }
        }

        public static void UseZaap(World.Network.WorldClient client, string packet)
        {
            int map = int.Parse(packet.Substring(2));
            Database.Records.ZaapRecord zaap = Helper.ZaapHelper.GetZaap(map);
            if (zaap != null)
            {
                int price = GetPriceOfTravel(map, client.Character.MapID);
                if (price <= client.Character.Kamas)
                {
                    client.Action.RemoveKamas(price);
                    Network.World.GoToMap(client, zaap.MapID, zaap.CellID);
                    CloseZaapPanel(client);
                }
                else
                {
                    client.SendImPacket("1128");
                }
            }
            else
            {
                client.Action.SystemMessage("Le zaap demander est inexistant !");
            }
        }

        public static void CloseZaapPanel(World.Network.WorldClient client, string packet = "")
        {
            client.State = Network.WorldClientState.None;
            client.Send("WV");
        }

        public static void SavePosition(World.Network.WorldClient client, int mapid)
        {
            Database.Records.ZaapRecord zaap = Helper.ZaapHelper.GetZaap(mapid);
            if (zaap != null)
            {
                if (client.Character.SaveMap == mapid)
                    return;
                client.Character.SaveMap = mapid;
                client.Character.SaveCell = zaap.CellID;
                client.SendImPacket("06");
                client.Character.SaveAndFlush();
            }
        }

    }
}
