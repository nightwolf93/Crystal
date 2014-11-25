using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.RealmServerReload.Utilities
{
    public class Logger
    {
        public static void DrawAscii()
        {

        }

        public static void Append(string header, string message, ConsoleColor headcolor)
        {
            Console.ForegroundColor = headcolor;
            Console.Write(header);
            Console.Write(" ");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var c in message)
            {
                if(c == '@')
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
            Console.Write("\n"); 
        }

        public static void Infos(string message)
        {
            Append("[Infos]", message, ConsoleColor.Green);
        }
    }
}
