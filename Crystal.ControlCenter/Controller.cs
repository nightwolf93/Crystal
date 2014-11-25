using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using SilverSock;

namespace Crystal.ControlCenter
{
    public class Controller
    {
        public delegate void LogDelegate(string message, Color color);
        public delegate void UpdateServersDelegate();

        public static int TempServerID = 1;
        public static SilverServer Server { get; set; }
        public static List<CrystalServer> ServersSpyed = new List<CrystalServer>();

        public static void Start()
        {
            Server = new SilverServer("5.135.187.100", 1574);
            Server.OnListeningEvent += new SilverEvents.Listening(Server_OnListeningEvent);
            Server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(Server_OnAcceptSocketEvent);
            Server.WaitConnection();
        }

        private static void Server_OnAcceptSocketEvent(SilverSocket socket)
        {
            LogIT("Nouveau serveur en observation <" + socket.IP + ">", Color.Green);
            ServersSpyed.Add(new CrystalServer(socket));
            TempServerID++;
        }

        private static void Server_OnListeningEvent()
        {
            LogIT("Serveur en ligne !", Color.Green);
        }

        public static void LogIT(string message, Color color)
        {
            Program.CenterForm.Invoke(new LogDelegate(Log), message, color);
        }

        public static void Log(string message, Color color)
        {
            Program.CenterForm.richTextBox1.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + message + "\n");
        }

        public static void UpdateServersIT()
        {
            Program.CenterForm.Invoke(new UpdateServersDelegate(UpdateServers));
        }

        public static void UpdateServers()
        {
            var grid = Program.CenterForm.dataGridView1;
            grid.Rows.Clear();

            foreach (var server in ServersSpyed)
            {
                grid.Rows.Add(server.ID, server.IP, server.Name, server.Version, server.PlayersCount, server.Uptime);
            }
        }
    }
}
