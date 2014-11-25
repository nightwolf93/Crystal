using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Crystal.WorldServer.World.Game.Ads
{
    public static class AdsManager
    {
        public static List<string> Ads = new List<string>();

        public static System.Timers.Timer SpawnAddTimer = new System.Timers.Timer(180000);

        public static void LoadAds()
        {
            var reader = new StreamReader("Datas/Ads.txt");
            while (!reader.EndOfStream)
            {
                Ads.Add(reader.ReadLine().Trim());
            }
            reader.Close();
            SpawnAddTimer.Enabled = true;
            SpawnAddTimer.Elapsed += SpawnAddTimer_Elapsed;
            SpawnAddTimer.Start();
        }

        private static void SpawnAddTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            World.Manager.WorldManager.SendMessage("<b>[PUB]</b> : " + RandomAd(), Utilities.ConfigurationManager.GetStringValue("AdsMessageColor"));
        }

        public static string RandomAd()
        {
            return Ads[Utilities.Basic.Rand(0, Ads.Count - 1)];
        }
    }
}
