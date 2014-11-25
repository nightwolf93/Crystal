using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : LicenceManager
*/

namespace Crystal.WorldServer.Security
{
    public class LicenceManager
    {
        public static string LicenceSipher { get; set; }
        public static string LicenceName { get; set; }

        public static bool LoadLicence()
        {
            try
            {
                if (!File.Exists("Datas/Licence.fwl"))
                {
                    return false;
                }
                StreamReader reader = new StreamReader("Datas/Licence.fwl");
                LicenceSipher = reader.ReadToEnd();
                reader.Close();
                return true;
            }
            catch { return false; }
        }

        public static bool CheckLicence()
        {
            try
            {
                WebClient web = new WebClient();
                WebRequest request = WebRequest.Create("http://crystal-project.free-h.net/secure.php?sid=" + LicenceSipher);
                WebResponse response = request.GetResponse();
                StreamReader streamResponse = new StreamReader(response.GetResponseStream());
                string strResponse = streamResponse.ReadLine();
                streamResponse.Close();
                string[] resData = strResponse.Split(';');
                string resID = resData[0];
                switch (resID)
                {
                    case "sid_ok":
                        LicenceName = resData[2];
                        return true;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
