using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public class Dispatcher
    {
        private WorldClient _client;

        public static Dictionary<string, MethodInfo> RegisteredMethods = new Dictionary<string, MethodInfo>();

        public Dispatcher(WorldClient client)
        {
            this._client = client;
        }

        public void Dispatch(string packet)
        {
            switch (packet[0])
            {
                case 'A':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("AA", packet);
                            break;

                        case 'B':
                            CallMethod("AB", packet);
                            break;

                        case 'D':
                            CallMethod("AD", packet);
                            break;

                        case 'T':
                            CallMethod("AT", packet);
                            break;

                        case 'P':
                            CallMethod("AP", packet);
                            break;

                        case 'S':
                            CallMethod("AS", packet);
                            break;

                        case 'L':
                            switch (packet[2])
                            {
                                case 'f':
                                    CallMethod("ALf", packet);
                                    break;
                            }
                            break;
                    }
                    break;

                case 'B':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("BA", packet);
                            break;

                        case 'a':
                            switch (packet[2])
                            {
                                case 'M':
                                    CallMethod("BaM", packet);
                                    break;
                            }
                            break;

                        case 'M':
                            CallMethod("BM", packet);
                            break;

                        case 'Y':
                            switch (packet[2])
                            {
                                case 'A':
                                    CallMethod("BYA", packet);
                                    break;
                            }
                            break;
                    }
                    break;

                case 'c':
                    switch (packet[1])
                    {
                        case 'C':
                            CallMethod("cC", packet);
                            break;
                    }
                    break;
                           

                case 'D':
                    switch (packet[1])
                    {
                        case 'C':
                            CallMethod("DC", packet);
                            break;

                        case 'R':
                            CallMethod("DR", packet);
                            break;

                        case 'V':
                            CallMethod("DV", packet);
                            break;
                    }
                    break;

                case 'E':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("EA", packet);
                            break;

                        case 'B':
                            CallMethod("EB", packet);
                            break;

                        case 'R':
                            CallMethod("ER", packet);
                            break;

                        case 'K':
                            CallMethod("EK", packet);
                            break;

                        case 'S':
                            CallMethod("ES", packet);
                            break;

                        case 'V':
                            CallMethod("EV", packet);
                            break;

                        case 'M':
                            switch (packet[2])
                            {
                                case 'O':
                                    CallMethod("EMO", packet);
                                    break;

                                case 'G':
                                    CallMethod("EMG", packet);
                                    break;
                            }
                            break;

                        case 'r':
                            switch (packet[2])
                            {
                                case 'C':
                                    CallMethod("ErC", packet);
                                    break;

                                case 'c':
                                    CallMethod("Erc", packet);
                                    break;

                                case 'g':
                                    CallMethod("Erg", packet);
                                    break;

                                case 'p':
                                    CallMethod("Erp", packet);
                                    break;
                            }
                            break;

                        case 'L':
                            CallMethod("EL", packet);
                            break;

                        case 'H':
                            switch (packet[2])
                            {
                                case 'T':
                                    CallMethod("EHT", packet);
                                    break;

                                case 'l':
                                    CallMethod("EHl", packet);
                                    break;

                                case 'B':
                                    CallMethod("EHB", packet);
                                    break;
                            }
                            break;
                    }
                    break;

                case 'e':
                    switch (packet[1])
                    {
                        case 'U':
                            CallMethod("eU", packet);
                            break;

                        case 'D':
                            CallMethod("eD", packet);
                            break;
                    }
                    break;

                case 'F':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("FA", packet);
                            break;

                        case 'D':
                            CallMethod("FD", packet);
                            break;

                        case 'L':
                            CallMethod("FL", packet);
                            break;
                    }
                    break;

                case 'f':
                    switch (packet[1])
                    {
                        case 'L':
                            CallMethod("fL", packet);
                            break;

                        case 'D':
                            CallMethod("fD", packet);
                            break;

                        case 'N':
                            CallMethod("fN", packet);
                            break;

                        case 'S':
                            CallMethod("fS", packet);
                            break;
                    }
                    break;

                case 'G':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("GA", packet);
                            break;

                        case 'C':
                            CallMethod("GC", packet);
                            break;

                        case 'Q':
                            CallMethod("GQ", packet);
                            break;

                        case 'R':
                            CallMethod("GR", packet);
                            break;

                        case 'P':
                            CallMethod("GP", packet);
                            break;

                        case 'f':
                            CallMethod("Gf", packet);
                            break;

                        case 'K':
                            switch (packet[2])
                            {
                                case 'K':
                                    CallMethod("GKK", packet);
                                    break;

                                case 'E':
                                    CallMethod("GKE", packet);
                                    break;
                            }
                            break;

                        case 'I':
                            CallMethod("GI", packet);
                            break;

                        case 't':
                            CallMethod("Gt", packet);
                            break;

                        case 'p':
                            CallMethod("Gp", packet);
                            break;
                    }
                    break;

                case 'g':
                    switch (packet[1])
                    {
                        case 'C':
                            CallMethod("gC", packet);
                            break;

                        case 'P':
                            CallMethod("gP", packet);
                            break;

                        case 'K':
                            CallMethod("gK", packet);
                            break;

                        case 'I':
                            switch (packet[2])
                            {
                                case 'M':
                                    CallMethod("gIM", packet);
                                    break;
                            }
                            break;

                        case 'J':
                            switch (packet[2])
                            {
                                case 'R':
                                    CallMethod("gJR", packet);
                                    break;

                                case 'E':
                                    CallMethod("gJE", packet);
                                    break;

                                case 'K':
                                    CallMethod("gJK", packet);
                                    break;
                            }
                            break;
                    }
                    break;

                case 'R':
                    switch (packet[1])
                    {
                        case 'r':
                            CallMethod("Rr", packet);
                            break;

                        case 'n':
                            CallMethod("Rn", packet);
                            break;
                    }
                    break;

                case 'i':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("iA", packet);    
                            break;
                            
                        case 'D':
                            CallMethod("iD", packet);
                            break;

                        case 'L':
                            CallMethod("iL", packet);
                            break;
                    }
                    break;

                case 'P':
                    switch (packet[1])
                    {
                        case 'A':
                            CallMethod("PA", packet);
                            break;

                        case 'R':
                            CallMethod("PR", packet);
                            break;

                        case 'I':
                            CallMethod("PI", packet);
                            break;

                        case 'V':
                            CallMethod("PV", packet);
                            break;
                    }
                    break;

                case 'S':
                    switch (packet[1])
                    {
                        case 'B':
                            CallMethod("SB", packet);
                            break;

                        case 'M':
                            CallMethod("SM", packet);
                            break;
                    }
                    break;

                case 'O':
                    switch (packet[1])
                    {
                        case 'd':
                            CallMethod("Od", packet);
                            break;

                        case 'D':
                            CallMethod("OD", packet);
                            break;

                        case 'M':
                            CallMethod("OM", packet);
                            break;

                        case 'U':
                            CallMethod("OU", packet);
                            break;
                    }
                    break;

                case 'W':
                    switch (packet[1])
                    {
                        case 'U':
                            CallMethod("WU", packet);
                            break;

                        case 'V':
                            CallMethod("WV", packet);
                            break;
                    }
                    break;
            }

            try
            {
                switch (packet.Split('|')[0])
                {
                    case "101REPORT":
                        Game.Reporting.ReportingManager.ReportingTicket(_client, packet);
                        break;
                }
            }
            catch (Exception e)
            {

            }

        }

        public void CallMethod(string header, string packet)
        {
            RegisteredMethods[header].Invoke(null, new object[] { _client, packet });
        }
    }
}
