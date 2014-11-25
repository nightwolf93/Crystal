using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Crystal.WorldServer.Globalization;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project Crystal.WorldServer

namespace Crystal.WorldServer
{
    public class Program
    {
        public static long StartTime = Environment.TickCount;
        public static long MaxIdleTime { get; set; }
        public const string CrystalVersion = "1.0.5.0";
        public const bool OnlyLocal = false;
        public const bool DebugMode = true;
        public const bool ARKALIA = true;
        public static ControlerForm AlternativeGui = new ControlerForm();
        public static string HelpContent { get; set; }
        public static int MaxConnected = 0;

        public static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;
                Console.Title = "crystal";
                Console.WindowWidth = 100;
                Console.WindowHeight = 50;
                Task.Factory.StartNew(() => Interop.StartArgsManager.ProcessArgs(args));
                Utilities.ConsoleStyle.DrawAscii();
                Utilities.ConsoleStyle.InitConsole();

                Utilities.ConfigurationManager.LoadConfiguration();
                MaxIdleTime = Utilities.ConfigurationManager.GetIntValue("MaximumIdleTime");
                StartTime = Environment.TickCount;
                Console.Title = "Crystal World " + Utilities.ConfigurationManager.GetStringValue("ServerName")
                                            + " (" + Utilities.ConfigurationManager.GetStringValue("ServerID") + ")";

                Globalization.I18nManager.LoadLangs();
                Utilities.ConsoleStyle.Infos(I18nManager.GetText(0));

                Utilities.ConsoleStyle.Infos(I18nManager.GetText(1));
                Database.Manager.DatabaseManager.StartDatabase();
                Utilities.ConsoleStyle.Infos(I18nManager.GetText(2));

                if (Utilities.ConfigurationManager.GetBoolValue("CreateShema"))
                {
                    Utilities.ConsoleStyle.Infos("Creating shema ...");
                    Database.Manager.DatabaseManager.InitTable();
                    Utilities.ConsoleStyle.Infos("Shema created !");
                }

                Utilities.ConsoleStyle.Infos(I18nManager.GetText(3));
                World.Handlers.AccountHandler.RegisterMethod();
                World.Handlers.GameHandler.RegisterMethod();
                World.Handlers.BasicHandler.RegisterMethod();
                World.Handlers.ItemHandler.RegisterMethod();
                World.Handlers.NpcHandler.RegisterMethod();
                World.Handlers.ZaapHandler.RegisterMethod();
                World.Handlers.FriendHandler.RegisterMethod();
                World.Handlers.EnemiesHandler.RegisterMethod();
                World.Handlers.DialogHandler.RegisterMethod();
                World.Handlers.ExchangeHandler.RegisterMethod();
                World.Handlers.PartyHandler.RegisterMethod();
                World.Handlers.SpellHandler.RegisterMethod();
                World.Handlers.FightHandler.RegisterMethod();
                World.Handlers.GuildHandler.RegisterMethod();
                World.Handlers.MountHandler.RegisterMethod();
                World.Handlers.EmoteHandler.RegisterMethod();
                Utilities.ConsoleStyle.Infos(I18nManager.GetText(4));

                Utilities.ConsoleStyle.Infos(I18nManager.GetText(5));
                World.Game.Jobs.JobManager.LoadJobs();
                Utilities.ConsoleStyle.Infos(I18nManager.GetText(6));

                Utilities.ConsoleStyle.Infos(I18nManager.GetText(7));
                Database.Cache.AccountDataCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.AccountDataCache.Cache.Count.ToString() + " @Accounts data loaded");

                Database.Cache.BreedCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.BreedCache.Cache.Count.ToString() + " @Breeds data loaded");

                Database.Cache.OriginalBreedStartMapCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.OriginalBreedStartMapCache.Cache.Count.ToString() + " @Original maps loaded");

                Database.Cache.IncarnamTeleporterCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.IncarnamTeleporterCache.Cache.Count.ToString() + " @Incarnam teleporters loaded");

                Database.Cache.GuildCreatorLocationCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.GuildCreatorLocationCache.Cache.Count.ToString() + " @Guild creator loaded");

                Database.Cache.GuildCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.GuildCache.Cache.Count.ToString() + " @Guild loaded");

                Utilities.ConsoleStyle.Infos("Loading @maps@ ...");
                Utilities.ConsoleStyle.EnableLoadingSymbol();
                Database.Cache.MapCache.Init();
                Utilities.ConsoleStyle.DisabledLoadingSymbol();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.MapCache.Cache.Count + " @Maps loaded !");

                Database.Cache.IODataCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.IODataCache.Cache.Count.ToString() + " @Interactive object loaded");

                Database.Cache.DungeonRoomCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.DungeonRoomCache.Cache.Count + " @Dungeon rooms loaded !");

                if (Utilities.ConfigurationManager.GetBoolValue("FastLoading"))
                {
                    Utilities.ConsoleStyle.Infos("Fast loading enabled !");
                    Utilities.ConsoleStyle.Warning("Be careful with this function, don't use it in real situation, use this only for debugging");
                    System.Threading.Thread triggerThread = new System.Threading.Thread(new System.Threading.ThreadStart(Database.Cache.TriggerCache.Init));
                    triggerThread.Start();
                }
                else
                {
                    Database.Cache.TriggerCache.Init();
                    Utilities.ConsoleStyle.Infos("@" + Database.Cache.TriggerCache.Cache.Count + " @Trigger loaded !");
                }

                Database.Cache.ExpFloorCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ExpFloorCache.Cache.Count + " @Exp floors loaded !");

                if (Utilities.ConfigurationManager.GetBoolValue("FastLoading"))
                {
                    System.Threading.Thread spellThread = new System.Threading.Thread(new System.Threading.ThreadStart(Database.Cache.SpellCache.Init));
                    spellThread.Start();
                }
                else
                {
                    Database.Cache.SpellCache.Init();
                    Utilities.ConsoleStyle.Infos("@" + Database.Cache.SpellCache.Cache.Count + " @Spells data loaded !");
                }

                Database.Cache.BaseSpellCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.BaseSpellCache.Cache.Count + " @Base spells loaded !");

                Database.Cache.NpcCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.NpcCache.Cache.Count + " @Npcs loaded !");

                Utilities.ConsoleStyle.Infos("Loading @characters@ ...");
                Utilities.ConsoleStyle.EnableLoadingSymbol();
                Database.Cache.CharacterCache.Init();
                Utilities.ConsoleStyle.DisabledLoadingSymbol();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.CharacterCache.Cache.Count + " @Characters loaded !");

                Database.Cache.MountTemplateCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.MountTemplateCache.Cache.Count + " @Mount templates loaded !");

                Database.Cache.WorldMountCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.WorldMountCache.Cache.Count + " @WorldMounts loaded !");

                Database.Cache.ItemCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ItemCache.Cache.Count + " @Items loaded !");

                Database.Cache.ItemSetCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ItemSetCache.Cache.Count + " @Item Sets loaded !");

                Utilities.ConsoleStyle.Infos("Loading @world items@ ...");
                Utilities.ConsoleStyle.EnableLoadingSymbol();
                Database.Cache.WorldItemCache.Init();
                Utilities.ConsoleStyle.DisabledLoadingSymbol();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.WorldItemCache.Cache.Count + " @WorldItems loaded !");

                Utilities.ConsoleStyle.Infos("Loading @item bags@ ...");
                Database.Cache.ItemBagCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ItemBagCache.Cache.Count + " @ItemBags loaded !");

                Database.Cache.NpcPositionCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.NpcPositionCache.Cache.Count + " @Npcs positions loaded !");

                Database.Cache.NpcDialogCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.NpcDialogCache.Cache.Count + " @Npcs dialogs loaded !");

                Database.Cache.MonstersTemplateCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.MonstersTemplateCache.Cache.Count + " @Monsters templates loaded !");

                Database.Cache.MonsterLevelCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.MonsterLevelCache.Cache.Count + " @Monsters levels loaded !");

                Database.Cache.DropCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.DropCache.Cache.Count + " @Drops loaded !");

                Database.Cache.ZaapCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ZaapCache.Cache.Count + " @Zaaps loaded !");

                Database.Cache.PaddockCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.PaddockCache.Cache.Count + " @Paddocks loaded !");

                Database.Cache.JobDataCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.JobDataCache.Cache.Count + " @Jobs loaded !");

                Database.Cache.CraftCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.CraftCache.Cache.Count + " @Recipes loaded !");

                Database.Cache.ShopItemCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ShopItemCache.Cache.Count + " @Shop item loaded !");

                Database.Cache.BannedAccountCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.BannedAccountCache.Cache.Count + " @Banned Account loaded !");

                Database.Cache.ElitesCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.ElitesCache.Cache.Count + " @Elites level loaded !");

                Database.Cache.HotelCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.HotelCache.Cache.Count + " @Hotel rooms loaded !");

                Database.Cache.AuctionHouseCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.AuctionHouseCache.Cache.Count + " @Auction Houses loaded !");

                Database.Cache.AuctionHouseItemsCache.Init();
                Utilities.ConsoleStyle.Infos("@" + Database.Cache.AuctionHouseItemsCache.Cache.Count + " @Auction Houses Items loaded !");

                Database.Cache.CharacterCache.SetToMaxLife();
                Utilities.ConsoleStyle.Infos("@Restoring character life@ completed !");
                Utilities.ConsoleStyle.Infos("Cache created !");

                Interop.Scripting.ScriptManager.Load("Scripts");
                Interop.PythonScripting.ScriptManager.Load();
                Utilities.ConsoleStyle.Infos("@Scripts@ loaded !");

                /* World contents enabling */

                World.Manager.WorldManager.SyncMonsterLevelWithTemplate();
                Utilities.ConsoleStyle.Infos("@Sync monsters@ with all levels available finished !");
                World.Manager.WorldManager.SyncMapWithMonsterAvailable();
                Utilities.ConsoleStyle.Infos("@Sync maps@ with all monsters available finished !");

                World.Manager.WorldManager.InitServer();
                Communication.Realm.Communicator.InitServer();
                Communication.Rcon.RConManager.InitServer();

                World.Network.World.InitAutoSave(Utilities.ConfigurationManager.GetIntValue("SaveIntervall"));
                Utilities.ConsoleStyle.Infos("@AutoSave@ started !");

                World.Network.World.InitAutoInformationsUpdate();

                World.Game.LiveActions.LiveActionManager.Initialize();
                Utilities.ConsoleStyle.Infos("@LiveActions@ initialized !");

                World.Game.Kolizeum.KolizeumManager.LoadMaps();
                Utilities.ConsoleStyle.Infos("@" + World.Game.Kolizeum.KolizeumManager.Maps.Count + "@ Kolizeum maps loaded !");
                World.Game.Kolizeum.KolizeumManager.LaunchKolizeumTask();

                World.Game.Shop.ShopManager.Logger = new Utilities.BasicLogger("Datas/Shop/shop_logs" + StartTime + ".log");
                LoadHelpFile();
                World.Game.Admin.AdminRankManager.Initialize();
                Utilities.ConsoleStyle.Infos("Admins ranks @permissions initialized@ !");

                World.Game.Pvp.PvpManager.LoadNoPvpMaps();
                World.Game.Exchange.ExchangeRestrictions.LoadRestrictedItems();
                World.Game.Ads.AdsManager.LoadAds();
                World.Game.Idle.IdleManager.Start();
                Utilities.ConsoleStyle.Infos("@" + World.Game.Ads.AdsManager.Ads.Count + "@ Ads loaded !");

                //World.Game.Pools.PoolManager.StartPoolsWarning();

                Utilities.ConsoleStyle.Infos("@Server online !@");
                Communication.NightWorld.NightWorldManager.Start();

                while (true)
                {
                    //TODO: Command scalar !
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Utilities.ConsoleStyle.Error("Error : " + ex.ToString());
                Console.ReadLine();
            }
        }

        public static void LoadHelpFile()
        {
            if (File.Exists("Datas/Help.txt"))
            {
                var r = new StreamReader("Datas/Help.txt");
                HelpContent = r.ReadToEnd();
                r.Close();
            }
            else
            {
                HelpContent = "null";
                Utilities.ConsoleStyle.Warning("No help file founded !");
            }
        }
    }
}
