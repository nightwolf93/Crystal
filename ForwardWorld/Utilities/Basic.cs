using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Utilities
{
    public class Basic
    {
        public static Random _random = new Random(Environment.TickCount);

        public static int Rand(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static bool IsNumeric(string data)
        {
            try
            {
                int.Parse(data);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static long GetPercentValue(int max, int percent)
        {
            return (max / 100) * percent;
        }

        public static bool GetFlag(int Number, int Flag)
        {
            switch (Flag)
            {
                case 0:
                    return (Number & 1) != 0;
                case 1:
                    return (Number & 2) != 0;
                case 2:
                    return (Number & 4) != 0;
                case 3:
                    return (Number & 8) != 0;
                case 4:
                    return (Number & 16) != 0;
                case 5:
                    return (Number & 32) != 0;
                case 6:
                    return (Number & 64) != 0;
                case 7:
                    return (Number & 128) != 0;
            }
            return false;
        }

        public static int Formulas_ExpPvp(int levelTeam, int levelRivalTeam)
        {
            int exp = 0;
            exp = Utilities.ConfigurationManager.GetIntValue("BaseCoefPvP") * Utilities.ConfigurationManager.GetIntValue("RatePvP");
            //exp = ((levelRivalTeam / levelTeam) * Utilities.ConfigurationManager.GetIntValue("BaseCoefPvP"))
            //            * Utilities.ConfigurationManager.GetIntValue("RatePvP");
            return exp;
        }

        public bool IsPair(int number)
        {
            if (((number >> 1) << 1) != number)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string MakeAccent(string Packet)
        {
            Packet = Packet.Replace("é", "\x00C3\x00A9");
            Packet = Packet.Replace("•", "â€¢");
            Packet = Packet.Replace("ê", "\x00C3\x00AA");
            Packet = Packet.Replace("Ê", "\x00C3\x008A");
            Packet = Packet.Replace("µ", "\x00C2\x00B5");
            Packet = Packet.Replace("É", "\x00C3\x0089");
            Packet = Packet.Replace("è", "\x00C3\x00A8");
            Packet = Packet.Replace("Ė", "\x00C4\x0096");
            Packet = Packet.Replace("â", "\x00C3\x00A2");
            Packet = Packet.Replace("ä", "\x00C3\x00A4");
            Packet = Packet.Replace("ç", "\x00C3\x00A7");
            Packet = Packet.Replace("ï", "\x00C3\x00AF");
            Packet = Packet.Replace("î", "\x00C3\x00AE");
            Packet = Packet.Replace("ù", "\x00C3\x00B9");
            Packet = Packet.Replace("ô", "\x00C3\x00B4");
            Packet = Packet.Replace("ö", "\x00C3\x00B6");
            Packet = Packet.Replace("Ô", "\x00C3\x0094");
            Packet = Packet.Replace("û", "\x00C3\x00BB");
            Packet = Packet.Replace("à", "\x00C3\x00A0");
            Packet = Packet.Replace("À", "Ã€");

            return Packet.Replace("Ã|", "Ã  |");
        }

        public static string GetUptime()
        {

            long Uptime = Environment.TickCount - Program.StartTime;
            string Time = "";

            if (Uptime >= 24 * 60 * 60 * 1000)
            {
                int Jours = 0;
                while (Uptime > 24 * 60 * 60 * 1000)
                {
                    Jours += 1;
                    Uptime -= 24 * 60 * 60 * 1000;
                }
                Time += Jours + "day" + (Jours > 1 ? "s" : "") + " ";
            }
            if (Uptime >= 60 * 60 * 1000)
            {
                int Hours = 0;
                while (Uptime > 60 * 60 * 1000)
                {
                    Hours += 1;
                    Uptime -= 60 * 60 * 1000;
                }
                Time += Hours + "h ";
            }
            if (Uptime >= 60 * 1000)
            {
                int Minutes = 0;
                while (Uptime > 60 * 1000)
                {
                    Minutes += 1;
                    Uptime -= 60 * 1000;
                }
                Time += Minutes + "m ";
            }
            if (Uptime >= 1000)
            {
                int Seconds = 0;
                while (Uptime > 1000)
                {
                    Seconds += 1;
                    Uptime -= 1000;
                }
                Time += Seconds + "s ";
            }

            int Millisecs = 0;
            while (Uptime > 0)
            {
                Millisecs += 1;
                Uptime -= 1;
            }
            Time += Millisecs + "ms";

            return Time;

        }
    }
}
