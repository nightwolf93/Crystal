using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Crystal.WorldServer.Graphics
{
    public class GraphicManager
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void HideConsole()
        {
            IntPtr hWnd = FindWindow(null, "crystal"); //put your console window caption here
            if (hWnd != IntPtr.Zero)
            {
                //Hide the window
                ShowWindow(hWnd, 0); // 0 = SW_HIDE
            }


            //if (hWnd != IntPtr.Zero)
            //{
            //    //Show window again
            //    ShowWindow(hWnd, 1); //1 = SW_SHOWNORMA
            //}
        }
    }
}
