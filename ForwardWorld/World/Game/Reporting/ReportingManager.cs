using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Reporting
{
    public class ReportingManager
    {
        public static List<Reporting> Reportings = new List<Reporting>();

        public static void SendOperatorOnline(World.Network.WorldClient client)
        {
            var packet = new StringBuilder("100MJ|");
            var names = new List<string>();
            World.Helper.WorldHelper.GetClientsArray.ToList().ForEach(x => names.Add(x.Character.Nickname));
            packet.Append(string.Join("|", names));
            names.Clear();
            client.Send(packet.ToString());
        }

        public static void ReportingTicket(World.Network.WorldClient client, string packet)
        {
            var data = packet.Split('|');
            var cate = data[1];
            var content = data[2];

            var report = new Reporting(client.Character.Nickname, cate, content);
            Reportings.Add(report);
        }
    }

    public class Reporting
    {
        public string Owner { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }

        public Reporting(string owner, string category, string content)
        {
            this.Owner = owner;
            this.Category = category;
            this.Content = content;
        }
    }
}
