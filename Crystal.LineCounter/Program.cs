using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crystal.LineCounter
{
    public class Program
    {
        private static int line = 0;

        public static void Main(string[] args)
        {
            Console.Title = "Program line counter :)";
            l("Calculate line dir (only .cs file) : ");
            var dir = Console.ReadLine();
            if (!Directory.Exists(dir))
            {
                l("dir not found !");
                return;
            }
            explore(dir);
            l("LINE COUNT : " + line);
            while (true) ;
        }

        private static void explore(string d)
        {
            l("explore : " + d);
            foreach (var f in Directory.GetFiles(d))
            {
                try
                {
                    var finfos = new FileInfo(f);
                    if (finfos.Extension != ".cs" && finfos.Extension != ".java")
                    {
                        continue;
                    }
                    l("read : " + f + "(" + finfos.Extension + ")");
                    var reader = new StreamReader(f);
                    while (!reader.EndOfStream)
                    {
                        var linestring = reader.ReadLine();
                        line++;
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    l("error : " + e.Message);
                }
            }
            foreach (var dir in Directory.GetDirectories(d))
            {
                explore(dir);
            }
        }

        private static void l(string m)
        {
            Console.WriteLine("[LOG] : " + m);
        }
    }
}
