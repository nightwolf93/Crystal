using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Crystal.WorldServer.Arkalia
{
    public class ArkaliaAPI
    {
        public static string APIURL = "http://109.236.87.106/api.php";
        public static string APIMDP = "56zdqs35623zef154316zefs231489";

        public static bool CanVote(string username)
        {
            try
            {
                WebClient web = new WebClient();
                WebRequest request = WebRequest.Create(APIURL + "?action=vote&username=" + username);
                WebResponse response = request.GetResponse();
                StreamReader streamResponse = new StreamReader(response.GetResponseStream());
                string strResponse = streamResponse.ReadLine();
                streamResponse.Close();
                string[] resData = strResponse.Split(';');
                string resID = resData[0];
                switch (resID)
                {
                    case "true":
                        return true;
                }
                return false;
            }
            catch { return false; }
        }

        public static bool IsValidAccount(string username, string password)
        {
            try
            {
                WebClient web = new WebClient();
                WebRequest request = WebRequest.Create(APIURL + "?action=account&username=" + username + "&password=" + password);
                WebResponse response = request.GetResponse();
                StreamReader streamResponse = new StreamReader(response.GetResponseStream());
                string strResponse = streamResponse.ReadLine();
                streamResponse.Close();
                string[] resData = strResponse.Split(';');
                string resID = resData[0];
                switch (resID)
                {
                    case "true":
                        return true;
                }
                return false;
            }
            catch { return false; }
        }

        public static void AddPoints(string username)
        {
            try
            {
                WebClient web = new WebClient();
                WebRequest request = WebRequest.Create(APIURL + "?action=addvote&username=" + username + "&mdp=" + APIMDP);
                WebResponse response = request.GetResponse();
            }
            catch { }
        }
    }
}
