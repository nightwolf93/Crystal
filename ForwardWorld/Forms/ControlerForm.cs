using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Crystal.WorldServer
{
    public partial class ControlerForm : Form
    {
        public ControlerForm()
        {
            InitializeComponent();
        }

        private void ControlerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void ControlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Environment.Exit(0);
            }
            catch (Exception ex) { }
        }

        private void ControlerForm_Load(object sender, EventArgs e)
        {

        }

        private void joueursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new Forms.PlayersListForm();
            frm.ShowDialog();
        }
    }
}
