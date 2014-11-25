using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Crystal.ControlCenter
{
    public static class Program
    {
        public static ControlCenterForm CenterForm { get; set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CenterForm = new ControlCenterForm();
            Application.Run(CenterForm);
        }
    }
}
