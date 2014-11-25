using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using MySql.Data.MySqlClient;
using Crystal.WorldServer.Database.Records;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Manager
{
    public static class DatabaseManager
    {
        public static void StartDatabase()
        {
            if (Utilities.ConfigurationManager.GetStringValue("SQLUsername") == "root")
            {
                Utilities.ConsoleStyle.Warning(@"'root' Username is not safe, please modify this for your security");
            }
            var config = new DatabaseConfiguration(DatabaseType.MySQL, Utilities.ConfigurationManager.GetStringValue("SQLHost"),
                                                    Utilities.ConfigurationManager.GetStringValue("SQLDB"),
                                                    Utilities.ConfigurationManager.GetStringValue("SQLUsername"), Utilities.ConfigurationManager.GetStringValue("SQLPassword"));

            var source = new InPlaceConfigurationSource();
            source.Add(typeof(ActiveRecordBase), config.GetProperties());

            ActiveRecordStarter.Initialize(source,
                            typeof(CharacterRecord), typeof(MapRecords),
                            typeof(TriggerRecord), typeof(ItemRecord), typeof(WorldItemRecord),
                            typeof(NpcRecord), typeof(NpcPositionRecord), typeof(NpcDialogRecord),
                            typeof(ZaapRecord), typeof(AccountDataRecord), typeof(OriginalBreedStartMapRecord),
                            typeof(IncarnamTeleportRecord), typeof(ExpFloorRecord), typeof(BaseSpellRecord),
                            typeof(MonstersTemplateRecord), typeof(MonsterLevelRecord), typeof(MountTemplateRecord),
                            typeof(GuildCreatorLocationRecord), typeof(SpellRecord), typeof(ShopItemRecord),
                            typeof(BreedRecord), typeof(GuildRecord), typeof(DungeonRoomRecord),
                            typeof(WorldMountRecord), typeof(PaddockRecord), typeof(DropRecord),
                            typeof(BannedAccountRecord), typeof(ElitesRecord), typeof(LiveActionRecord),
                            typeof(HotelRecord), typeof(ServerInfoRecord), typeof(ItemSetRecord),
                            typeof(IODataRecord), typeof(JobDataRecord), typeof(CraftRecord), typeof(ItemBagRecord),
                            typeof(AuctionHouseRecord), typeof(AuctionHouseItemRecord));
        }

        public static void InitTable()
        {
            ActiveRecordStarter.CreateSchema();
        }

        public enum DatabaseType
        {
            MySQL,
            MSSQL2005,
            MSSQL2008
        }

        public class DatabaseConfiguration
        {
            private readonly IDictionary<string, string> m_props;

            public DatabaseConfiguration(DatabaseType dbtype, string host, string dbName, string user, string password)
            {
                m_props = new Dictionary<string, string>();

                switch (dbtype)
                {
                    case DatabaseType.MySQL:
                        {
                            m_props.Add("connection.driver_class", "NHibernate.Driver.MySqlDataDriver");
                            m_props.Add("dialect", "NHibernate.Dialect.MySQLDialect");
                            m_props.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                            m_props.Add("connection.connection_string", "Database=" + dbName + ";Data Source=" + host +
                                                                        ";User Id=" + user + ";Password=" + password);
                            m_props.Add("proxyfactory.factory_class",
                                        "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                            break;
                        }
                    case DatabaseType.MSSQL2005:
                        {
                            m_props.Add("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
                            m_props.Add("dialect", "NHibernate.Dialect.MsSql2005Dialect");
                            m_props.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                            m_props.Add("connection.connection_string",
                                        "Data Source=" + host + ";Initial Catalog=" + dbName + ";User Id=" + user +
                                        ";Password=" + password + ";");
                            m_props.Add("proxyfactory.factory_class",
                                        "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                            break;
                        }
                    case DatabaseType.MSSQL2008:
                        {
                            m_props.Add("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
                            m_props.Add("dialect", "NHibernate.Dialect.MsSql2008Dialect");
                            m_props.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                            m_props.Add("connection.connection_string",
                                        "Data Source=" + host + ";Initial Catalog=" + dbName + ";User Id=" + user +
                                        ";Password=" + password + ";");
                            m_props.Add("proxyfactory.factory_class",
                                        "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                            break;
                        }
                }
            }

            public IDictionary<string, string> GetProperties()
            {
                return m_props;
            }
        }
    }
}
