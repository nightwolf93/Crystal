using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Forward.ExpFloorGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //'INSERT INTO exp_floors(ID,Characters) VALUES("'.$level.'", "'.$xp.'");
            long baseExp = long.Parse(textBox1.Text);
            long baseFloor = long.Parse(textBox2.Text);
            long baseCoeft = long.Parse(textBox3.Text);
            float baseDividande = float.Parse(textBox4.Text);

            double currentCoeft = baseCoeft;
            double currentFloor = baseExp;

            for (int i = 200; i <= 5000; i++)
            {
                currentCoeft = Math.Truncate(currentCoeft * baseDividande);
                currentFloor = currentFloor + currentCoeft;

                richTextBox1.AppendText("INSERT INTO exp_floors(ID,Characters) VALUES('" + i + "','" + currentFloor + "');\n");
            }
        }
    }
}
