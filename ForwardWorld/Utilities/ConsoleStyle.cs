using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Crystal.WorldServer.Utilities
{
    public static class ConsoleStyle
    {
        public static object Locker = new object();
        public static string CurrentLoadingSymbol = "|";
        public static Timer LoadingSymbolTimer = new Timer(50);

        public delegate void WriteMessageDelegate(string message, System.Drawing.Color color);

        public static void InitConsole()
        {
            LoadingSymbolTimer.Elapsed += new ElapsedEventHandler(LoadingSymbolTimer_Elapsed);
        }

        public static void EnableLoadingSymbol()
        {
            LoadingSymbolTimer.Enabled = true;
            LoadingSymbolTimer.Start();
            Console.Write(" " + CurrentLoadingSymbol);
        }

        public static void DisabledLoadingSymbol()
        {
            try
            {
                LoadingSymbolTimer.Enabled = false;
                LoadingSymbolTimer.Stop();
                LoadingSymbolTimer.Close();
                Console.CursorLeft -= 1;
                Console.Write(" ");
            }
            catch { }
        }

        private static void LoadingSymbolTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.CursorLeft -= 1;
                switch (CurrentLoadingSymbol)
                {
                    case "|":
                        CurrentLoadingSymbol = "/";
                        break;

                    case "/":
                        CurrentLoadingSymbol = "-";
                        break;

                    case "-":
                        CurrentLoadingSymbol = "\\";
                        break;

                    case "\\":
                        CurrentLoadingSymbol = "|";
                        break;
                }
                Console.Write(CurrentLoadingSymbol);
            }
            catch { }
        }

        public static void DrawAscii()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"                      _____                _        _ ");
            Console.WriteLine(@"                     / ____|              | |      | |");
            Console.WriteLine(@"                    | |     _ __ _   _ ___| |_ __ _| |");
            Console.WriteLine(@"                    | |    | '__| | | / __| __/ _` | |      By NightWolf v" + Program.CrystalVersion);
            Console.WriteLine(@"                    | |____| |  | |_| \__ \ || (_| | |      http://nightwolf.fr/");
            Console.WriteLine(@"                     \_____|_|   \__, |___/\__\__,_|_|");
            Console.WriteLine(@"                                  __/ |               ");
            Console.WriteLine(@"                                 |___/                ");

            Console.WriteLine(@" __________________________________________________________________________________________________");
            Console.WriteLine(@"");
        }

        public static void Append(string header, string message, ConsoleColor headcolor)
        {
            lock (Locker)
            {
                Console.Write("\n");
                Console.ForegroundColor = headcolor;
                Console.Write(header);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var c in message)
                {
                    if (c == '@')
                    {
                        if (Console.ForegroundColor == ConsoleColor.Gray)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        Console.Write(c);
                    }
                }
            }
        }

        public static void Infos(string message)
        {
            Append("[Infos]", message, ConsoleColor.Green);
            try
            {
                if (Program.AlternativeGui.Visible)
                    Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.Green);
            }
            catch { }

        }

        public static void Error(string message)
        {
            Append("[Error]", message, ConsoleColor.Red);
            try
            {
                if (Program.AlternativeGui.Visible)
                    Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.Red);
            }
            catch { }
        }

        public static void Debug(string message)
        {
            if (Program.DebugMode)
            {
                Append("[Debug]", message, ConsoleColor.Magenta);
                try
                {
                    if (Program.AlternativeGui.Visible)
                        Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.Purple);
                }
                catch { }
            }
        }

        public static void Warning(string message)
        {
            try
            {
                Append("[Warning]", message, ConsoleColor.Yellow);
                if (Program.AlternativeGui.Visible)
                    Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.Orange);
            }
            catch { }
        }

        public static void Script(string message, string script)
        {
            try
            {
                Append("[(" + script + ") Script]", message, ConsoleColor.Yellow);
                if (Program.AlternativeGui.Visible)
                    Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.Orange);
            }
            catch { }
        }

        public static void Realm(string message)
        {
            try
            {
                Append("[Realm]", message, ConsoleColor.DarkGreen);
                if (Program.AlternativeGui.Visible)
                    Program.AlternativeGui.Invoke(new WriteMessageDelegate(WriteGui), message, System.Drawing.Color.DarkGreen);
            }
            catch { }
        }

        public static void WriteGui(string message, System.Drawing.Color color)
        {
            message = message.Replace("@", "");
            var box = Program.AlternativeGui.richTextBox1;
            box.AppendText(message + "\n");
            box.SelectionStart = box.Find(message, System.Windows.Forms.RichTextBoxFinds.Reverse);
            box.SelectionColor = color;
            box.DeselectAll();
            box.ScrollToCaret();
        }
    }
}
