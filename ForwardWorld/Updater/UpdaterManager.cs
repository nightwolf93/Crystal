using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Crystal.WorldServer.Updater
{
    public class UpdaterManager
    {
        public const string URL_ROOT = "http://91.229.20.41/updater/update.txt";

        public static bool UpdateIsNeeded()
        {
            Utilities.ConsoleStyle.Infos("Checking your Crystal version ...");
            var client = new WebClient();
            client.DownloadFile(URL_ROOT, "update.txt");
            var updateFile = new Utilities.IniSetting("update.txt");
            updateFile.ReadSettings();
            File.Delete("update.txt");
            if (!updateFile.ContainsGroup(Program.CrystalVersion))
            {
                try
                {
                    Utilities.ConsoleStyle.Warning("We downloading the last version, please wait ...");
                    client.DownloadFile(updateFile.GetFirstGroup()["Update_lnk"], "update.zip");
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("The update service is not available for this moment please try later !");
                    System.Threading.Thread.Sleep(2500);
                    return false;
                }
                return true;
            }

            Utilities.ConsoleStyle.Infos("Your version is up to day !");
            return false;
        }
    }
}
