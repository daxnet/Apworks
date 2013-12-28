using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Reflection;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Bus.DirectBus;
using Apworks.Bus.MSMQ;
using Apworks.Config;
using Apworks.Events.Storage;
using Apworks.Events.Storage.General;
using Apworks.Events.Storage.MySql;
using Apworks.ObjectContainers.Unity;
using Apworks.Repositories;
using Apworks.Repositories.EntityFramework;
using Apworks.Repositories.MongoDB;
using Apworks.Repositories.NHibernate;
using Apworks.Snapshots.Providers;
using Apworks.Storage;
using Apworks.Storage.General;
using Apworks.Tests.Common.AggregateRoots;
using Apworks.Tests.Common.EFContexts;
using Apworks.Tests.Common.Events;
using Apworks.Tests.Common.MongoDB;
using Microsoft.Practices.Unity;
using MySql.Data.MySqlClient;
using MongoDB.Driver;

namespace Apworks.Tests.Common
{
    public static class Helper
    {
        #region Constants
        //public const string CQRSTestDB_SQLExpress_ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=ApworksCQRSArchEventStoreTestDB;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;";
        public const string CQRSTestDB_SQLExpress_ConnectionString = @"Server=localhost;Database=ApworksCQRSArchEventStoreTestDB;Integrated Security=SSPI;";
        public const string CQRSTestDB_MySql_ConnectionString = @"server=localhost;database=apworkscqrsarcheventstoretestdb;uid=root;pwd=P@ssw0rd";
        public const string ClassicTestDB_SQLExpress_ConnectionString = @"Data Source=localhost;Initial Catalog=ApworksClassicArchTestDB;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;";
        public const string CQRSTestDB_Table_DomainEvents = @"DomainEvents";
        public const string CQRSTestDB_Table_Snapshots = @"Snapshots";
        public const string CQRSTestDB_Table_SourcedCustomer = @"SourcedCustomer";
        public const string EventBus_MessageQueue = @"EventBusMQ";
        public const string CommandBus_MessageQueue = @"CommandBusMQ";
        //public const string EF_SQL_ConnectionString = "Server=localhost; Database=EFTestContext; Integrated Security=True; MultipleActiveResultSets=True;";
        public const string EF_SQL_ConnectionString = @"Server=localhost; Database=EFTestContext; Integrated Security=True; MultipleActiveResultSets=True;";
        public const string EF_Table_EFCustomers = @"EFCustomers";
        public const string EF_Table_EFNotes = @"EFNotes";
        public const string EF_Table_EFCustomerNotes = @"EFCustomerNotes";

        public const string MongoDB_Database = @"ApworksMongoTest";
        public const string MongoDB_ConnectionString = @"mongodb://localhost/?safe=true";
        #endregion

        public static readonly Guid AggregateRootId1 = new Guid("{79C5DEAE-98E1-4815-9390-D5002980D987}");
        public static readonly Guid AggregateRootId2 = new Guid("{3B4EAF59-F8F5-4E07-971B-0DB65273F9A1}");
        public static readonly Guid AggregateRootId3 = new Guid("{3D1E71F0-BF7D-4921-A0F9-B712CC95D07D}");

        public static readonly Guid Id1 = new Guid("{AA68BAD6-0871-4007-ABFF-A455B7097F01}");
        public static readonly Guid Id2 = new Guid("{86F3D0C7-3135-4197-94BA-CB9E22149FD3}");
        public static readonly Guid Id3 = new Guid("{BF768E69-0F08-41F7-8CE7-EFEA5BA252B3}");

        #region Sample Data
        public static CreateCustomerDomainEvent[] CreateCreateCustomerDomainEvents()
        {
            var sourcedCustomer1 = new SourcedCustomer { ID = AggregateRootId1 };
            var sourcedCustomer2 = new SourcedCustomer { ID = AggregateRootId2 };
            var sourcedCustomer3 = new SourcedCustomer { ID = AggregateRootId3 };
            var createCustomerEvents = new CreateCustomerDomainEvent[]
            {
                new CreateCustomerDomainEvent()
                {
                    ID = Id1,
                    Branch = 1,
                    Username = "sunny chen",
                    Timestamp = DateTime.Now,
                    Version = 3,
                    Source = sourcedCustomer1
                },
                new CreateCustomerDomainEvent()
                {
                    ID = Id2,
                    Branch = 1,
                    Username = "daxnet",
                    Timestamp = DateTime.Now,
                    Version = 1,
                    Source = sourcedCustomer2
                },
                new CreateCustomerDomainEvent()
                {
                    ID = Id3,
                    Branch = 1,
                    Username = "acqy",
                    Timestamp = DateTime.Now,
                    Version = 2,
                    Source = sourcedCustomer3
                }
            };
            return createCustomerEvents;
        }
        #endregion

        #region Instrumentation

        private static string CreateMessageQueueName(string mq)
        {
            return string.Format(@".\private$\{0}", mq);
        }

        public static void WriteTempFile(string content)
        {
            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            string fileName = Path.Combine(tempPath, "apworks.txt");
            File.WriteAllText(fileName, content);
        }

        public static bool IsTempFileExists()
        {
            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            string fileName = Path.Combine(tempPath, "apworks.txt");
            return File.Exists(fileName);
        }

        public static string ReadTempFile()
        {
            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            string fileName = Path.Combine(tempPath, "apworks.txt");
            return File.ReadAllText(fileName);
        }

        public static void DeleteTempFile()
        {
            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            string fileName = Path.Combine(tempPath, "apworks.txt");
            File.Delete(fileName);
        }

        public static void ClearCQRSSQLExpressTestDB()
        {
            using (SqlConnection conn = new SqlConnection(CQRSTestDB_SQLExpress_ConnectionString))
            {
                conn.Open();
                List<string> tablesToClear = new List<string> { CQRSTestDB_Table_DomainEvents, CQRSTestDB_Table_Snapshots, CQRSTestDB_Table_SourcedCustomer };
                foreach (var table in tablesToClear)
                {
                    using (SqlCommand command = new SqlCommand(string.Format("DELETE FROM {0}", table), conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
        }

        public static void ClearCQRSMySqlTestDB()
        {
            using (MySqlConnection conn = new MySqlConnection(CQRSTestDB_MySql_ConnectionString))
            {
                conn.Open();
                List<string> tablesToClear = new List<string> { CQRSTestDB_Table_DomainEvents, CQRSTestDB_Table_Snapshots };
                foreach (var table in tablesToClear)
                {
                    using (MySqlCommand command = new MySqlCommand(string.Format("DELETE FROM {0}", table), conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
        }

        public static void ClearEFTestDB()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(EF_SQL_ConnectionString))
                {
                    conn.Open();
                    List<string> tablesToClear = new List<string> { EF_Table_EFCustomerNotes, EF_Table_EFNotes, EF_Table_EFCustomers };
                    foreach (var table in tablesToClear)
                    {
                        using (SqlCommand command = new SqlCommand(string.Format("DELETE FROM {0}", table), conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
            catch { }
        }

        public static int ReadRecordCountFromSQLExpressCQRSTestDB(string table)
        {
            int ret = 0;
            using (SqlConnection conn = new SqlConnection(CQRSTestDB_SQLExpress_ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT COUNT(*) FROM {0}", table), conn))
                {
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return ret;
        }

        public static int ReadRecordCountFromMySqlCQRSTestDB(string table)
        {
            int ret = 0;
            using (MySqlConnection conn = new MySqlConnection(CQRSTestDB_MySql_ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0}", table), conn))
                {
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return ret;
        }

        public static int ReadRecordCountFromEFTestDB(string table)
        {
            int ret = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(EF_SQL_ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(string.Format("SELECT COUNT(*) FROM {0}", table), conn))
                    {
                        ret = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    conn.Close();
                }
            }
            catch { }
            return ret;
        }

        public static DataTable ReadRecordsFromSQLExpressCQRSTestDB(string table, string w = "")
        {
            DataTable dataTable = new DataTable();
            string sql = string.Format("SELECT * FROM {0}", table);
            if (!string.IsNullOrEmpty(w))
                sql += " WHERE " + w;
            using (SqlConnection conn = new SqlConnection(CQRSTestDB_SQLExpress_ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
                conn.Close();
            }
            return dataTable;
        }

        public static DataTable ReadRecordsFromMySqlCQRSTestDB(string table, string w = "")
        {
            DataTable dataTable = new DataTable();
            string sql = string.Format("SELECT * FROM {0}", table);
            if (!string.IsNullOrEmpty(w))
                sql += " WHERE " + w;
            using (MySqlConnection conn = new MySqlConnection(CQRSTestDB_MySql_ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
                conn.Close();
            }
            return dataTable;
        }

        public static void ClearMessageQueue(string queue)
        {
            string mqName = CreateMessageQueueName(queue);
            if (!MessageQueue.Exists(mqName))
                MessageQueue.Create(mqName, true);
            else
            {
                using (MessageQueue mq = new MessageQueue(mqName, false, false, QueueAccessMode.SendAndReceive))
                {
                    mq.Purge();
                    mq.Close();
                }
            }
        }

        public static int GetMessageQueueCount(string queue)
        {
            string mqName = CreateMessageQueueName(queue);
            if (!MessageQueue.Exists(mqName))
            {
                MessageQueue.Create(mqName, true);
                return 0;
            }
            else
            {
                int ret = 0;
                using (MessageQueue mq = new MessageQueue(mqName, false, false, QueueAccessMode.SendAndReceive))
                {
                    ret = mq.GetAllMessages().Length;
                    mq.Close();
                }
                return ret;
            }
        }

        public static Message[] GetAllMessages(string queue)
        {
            string mqName = CreateMessageQueueName(queue);
            if (!MessageQueue.Exists(mqName))
            {
                MessageQueue.Create(mqName, true);
                return null;
            }
            else
            {
                Message[] ret = null;
                using (MessageQueue mq = new MessageQueue(mqName, false, false, QueueAccessMode.SendAndReceive))
                {
                    ret = mq.GetAllMessages();
                    mq.Close();
                }
                return ret;
            }
        }

        public static void ClearApp(AppRuntime appRuntime)
        {
            typeof(AppRuntime).GetField("currentApplication",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(appRuntime, null);
        }

        public static void ClearMongoDB()
        {
            var mongoClient = new MongoClient(MongoDB_ConnectionString);
            //MongoServer server = MongoServer.Create(MongoDB_ConnectionString);
            var server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase(MongoDB_Database);
            database.Drop();
            server.Disconnect();
        }
        #endregion

        #region Configuration Sources
        public static IConfigSource ConfigSource_EventStore_MySql
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();

                configSource.Application = typeof(App);
                configSource.EventSerializer = typeof(Apworks.Events.Serialization.DomainEventXmlSerializer);
                configSource.SnapshotSerializer = typeof(Apworks.Snapshots.Serialization.SnapshotXmlSerializer);
                configSource.ObjectContainer = typeof(UnityObjectContainer);
                configSource.InitObjectContainerFromConfigFile = false;
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);

                return configSource;
            }
        }

        public static IConfigSource ConfigSource_EventStore_SqlExpress
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();

                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.EventSerializer = typeof(Apworks.Events.Serialization.DomainEventXmlSerializer);
                configSource.SnapshotSerializer = typeof(Apworks.Snapshots.Serialization.SnapshotXmlSerializer);
                configSource.InitObjectContainerFromConfigFile = false;
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);

                return configSource;
            }
        }

        public static IConfigSource ConfigSource_ExceptionHandling
        {
            get
            {
                var methods = typeof(SqlStorage).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var method = methods.Where(p =>
                    p.Name == "Select" &&
                    p.IsGenericMethod == true &&
                    p.GetGenericArguments().Length == 1 &&
                    p.ReturnType == typeof(IEnumerable<>).MakeGenericType(p.GetGenericArguments()[0]) &&
                    p.GetParameters().Length == 0
                    ).First();
                RegularConfigSource configSource = new RegularConfigSource();
                
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.AddException(typeof(System.InvalidOperationException), ExceptionHandlingBehavior.Direct);
                configSource.AddException(typeof(System.ArgumentNullException));
                configSource.AddException(typeof(System.ArgumentException), ExceptionHandlingBehavior.Forward);
                configSource.AddException(typeof(System.SystemException));
                configSource.AddException(typeof(System.Exception));
                configSource.AddExceptionHandler(typeof(System.InvalidOperationException), typeof(Apworks.Tests.Common.ExceptionHandlers.InvalidOperationExceptionHandler));
                configSource.AddExceptionHandler(typeof(System.SystemException), typeof(Apworks.Tests.Common.ExceptionHandlers.SystemExceptionHandler));
                configSource.AddExceptionHandler(typeof(System.Exception), typeof(Apworks.Tests.Common.ExceptionHandlers.ExceptionExceptionHandler));

                configSource.AddInterceptor("exception", typeof(Interception.ExceptionHandlingInterceptor));
                configSource.AddInterceptorRef(typeof(SqlStorage), method, "exception");
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Generators
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_NHibernateRepository
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.AddInterceptor("exception_handling", typeof(Apworks.Interception.ExceptionHandlingInterceptor));
                configSource.AddInterceptorRef(typeof(Apworks.IUnitOfWork), typeof(Apworks.IUnitOfWork).GetMethod("Commit"), "exception_handling");

                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_SnapshotDomainRepository_DirectBus
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_SnapshotDomainRepository_MSMQ
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_SnapshotDomainRepository_SaveButFailPubToMSMQ
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_RegularEventPublisherDomainRepository_MSMQ
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Buses_EventSourcedDomainRepositoryWithDirectCommandBusButWithoutSnapshotProvider
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.AddHandler("CommandHandlers", HandlerKind.Command, HandlerSourceType.Assembly, "Apworks.Tests.Buses");
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_DomainEvents
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_AggregateRootVersion
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_RegularDomainRepository
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.IdentityGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                configSource.SequenceGenerator = typeof(Apworks.Generators.SequentialIdentityGenerator);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_MessageDispatcher
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                configSource.AddHandler("createCustomerCommand", HandlerKind.Command, HandlerSourceType.Assembly, typeof(Helper).Assembly.FullName);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_GeneralInterception
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_EFRepository
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }

        public static IConfigSource ConfigSource_MongoDBRepository
        {
            get
            {
                RegularConfigSource configSource = new RegularConfigSource();
                configSource.Application = typeof(App);
                configSource.ObjectContainer = typeof(Apworks.ObjectContainers.Unity.UnityObjectContainer);
                return configSource;
            }
        }
        #endregion

        #region Application Initialize Delegates
        public static void AppInit_ExceptionHandling_InvalidStorage(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("ExceptionHandlingInvalidStorage.xml"))
                .RegisterType<IStorage, SqlStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()));
        }
        public static void AppInit_EventStore_MySql(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>("DomainEventStorageMappingResolver", new InjectionConstructor("StorageMappings.xml"))
                .RegisterType<IDomainEventStorage, MySqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_MySql_ConnectionString, new ResolvedParameter<IStorageMappingResolver>("DomainEventStorageMappingResolver")));
        }

        public static void AppInit_EventStore_SqlExpress(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>("DomainEventStorageMappingResolver", new InjectionConstructor("StorageMappings.xml"))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>("DomainEventStorageMappingResolver")));
        }

        public static void AppInit_Repositories_NHibernateRepository(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            NHibernate.Cfg.Configuration nhibernateCfg = new NHibernate.Cfg.Configuration();

            nhibernateCfg.Properties["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
            nhibernateCfg.Properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            nhibernateCfg.Properties["connection.connection_string"] = ClassicTestDB_SQLExpress_ConnectionString;
            nhibernateCfg.Properties["dialect"] = "NHibernate.Dialect.MsSql2008Dialect";
            nhibernateCfg.Properties["proxyfactory.factory_class"] = "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate";
            //nhibernateCfg.Properties["linqtohql.generatorsregistry"] ="Apworks.Repositories.NHibernate.ExtendedLinqToHqlGeneratorsRegistry, Apworks.Repositories.NHibernate";
            nhibernateCfg.AddAssembly(typeof(Customer).Assembly);

            c.RegisterInstance<NHibernate.Cfg.Configuration>(nhibernateCfg)
                .RegisterType<IRepositoryContext, NHibernateContext>(new InjectionConstructor(new ResolvedParameter<NHibernate.Cfg.Configuration>()))
                //.RegisterType<IRepository<Customer>, NHibernateRepository<Customer>>(new InjectionConstructor(new ResolvedParameter<IRepositoryContext>()));
                .RegisterType<IRepository<Customer>, NHibernateRepository<Customer>>();
            
        }

        public static void AppInit_Repositories_SnapshotDomainRepository_DirectBus(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IMessageDispatcher, MessageDispatcher>()
                .RegisterType<IEventBus, DirectEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IMessageDispatcher>()))
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("SnapshotDomainRepository_StorageMappings.xml"))
                .RegisterType<IStorage, SqlStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IDomainRepository, SnapshotDomainRepository>(new InjectionConstructor(new ResolvedParameter<IStorage>(), new ResolvedParameter<IEventBus>()));
        }

        public static void AppInit_Repositories_SnapshotDomainRepository_MSMQ(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(Helper.CreateMessageQueueName(EventBus_MessageQueue), false))
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("SnapshotDomainRepository_StorageMappings.xml"))
                .RegisterType<IStorage, SqlStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IDomainRepository, SnapshotDomainRepository>(new InjectionConstructor(new ResolvedParameter<IStorage>(), new ResolvedParameter<IEventBus>()));
        }

        public static void AppInit_Repositories_SnapshotDomainRepository_SaveButFailPubToMSMQ(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IMessageDispatcher, MessageDispatcher>()
                .RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(EventBus_MessageQueue, false)) // register an invalid queue here
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("SnapshotDomainRepository_StorageMappings.xml"))
                .RegisterType<IStorage, SqlStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IDomainRepository, SnapshotDomainRepository>(new InjectionConstructor(new ResolvedParameter<IStorage>(), new ResolvedParameter<IEventBus>()));
        }

        public static void AppInit_Repositories_RegularEventPublisherDomainRepository_MSMQ(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();

            NHibernate.Cfg.Configuration nhibernateCfg = new NHibernate.Cfg.Configuration();
            nhibernateCfg.Properties["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
            nhibernateCfg.Properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            nhibernateCfg.Properties["connection.connection_string"] = CQRSTestDB_SQLExpress_ConnectionString;
            nhibernateCfg.Properties["dialect"] = "NHibernate.Dialect.MsSql2008Dialect";
            nhibernateCfg.Properties["proxyfactory.factory_class"] = "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate";
            //nhibernateCfg.Properties["linqtohql.generatorsregistry"] = "Apworks.Repositories.NHibernate.ExtendedLinqToHqlGeneratorsRegistry, Apworks.Repositories.NHibernate";
            nhibernateCfg.AddAssembly(typeof(SourcedCustomer).Assembly);

            c.RegisterInstance<NHibernate.Cfg.Configuration>(nhibernateCfg)
                .RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(Helper.CreateMessageQueueName(EventBus_MessageQueue), true))
                .RegisterType<IRepositoryContext, NHibernateContext>(new InjectionConstructor(new ResolvedParameter<NHibernate.Cfg.Configuration>()))
                .RegisterType<IRepository<SourcedCustomer>, NHibernateRepository<SourcedCustomer>>()
                .RegisterType<IDomainRepository, RegularEventPublisherDomainRepository>(new InjectionConstructor(new ResolvedParameter<IRepositoryContext>(), new ResolvedParameter<IEventBus>()));
        }

        public static void AppInit_Repositories_RegularDomainRepository(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();

            NHibernate.Cfg.Configuration nhibernateCfg = new NHibernate.Cfg.Configuration();
            nhibernateCfg.Properties["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
            nhibernateCfg.Properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            nhibernateCfg.Properties["connection.connection_string"] = CQRSTestDB_SQLExpress_ConnectionString;
            nhibernateCfg.Properties["dialect"] = "NHibernate.Dialect.MsSql2008Dialect";
            nhibernateCfg.Properties["proxyfactory.factory_class"] = "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate";
            //nhibernateCfg.Properties["linqtohql.generatorsregistry"] = "Apworks.Repositories.NHibernate.ExtendedLinqToHqlGeneratorsRegistry, Apworks.Repositories.NHibernate";
            nhibernateCfg.AddAssembly(typeof(SourcedCustomer).Assembly);

            c.RegisterInstance<NHibernate.Cfg.Configuration>(nhibernateCfg)
                //.RegisterType<IRepositoryContext, NHibernateContext>(new InjectionConstructor(new ResolvedParameter<NHibernate.Cfg.Configuration>()))
                .RegisterType<IRepositoryContext, NHibernateContext>(new InjectionConstructor(nhibernateCfg))
                .RegisterType<IRepository<SourcedCustomer>, NHibernateRepository<SourcedCustomer>>()
                .RegisterType<IDomainRepository, RegularDomainRepository>(new InjectionConstructor(new ResolvedParameter<IRepositoryContext>()));
        }

        public static void AppInit_Repositories_EventSourcedDomainRepositoryWithDirectEventBusButWithoutSnapshotProvider(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IMessageDispatcher, MessageDispatcher>()
                .RegisterType<IEventBus, DirectEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IMessageDispatcher>()))
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("EventSourcedRepository_StorageMappings.xml"))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<ISnapshotProvider, SuppressedSnapshotProvider>()
                .RegisterType<IDomainRepository, EventSourcedDomainRepository>(new InjectionConstructor(new ResolvedParameter<IDomainEventStorage>(), new ResolvedParameter<IEventBus>(), new ResolvedParameter<ISnapshotProvider>()));
        }

        public static void AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(Helper.CreateMessageQueueName(EventBus_MessageQueue), true))
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("EventSourcedRepository_StorageMappings.xml"))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<ISnapshotProvider, SuppressedSnapshotProvider>()
                .RegisterType<IDomainRepository, EventSourcedDomainRepository>(new InjectionConstructor(new ResolvedParameter<IDomainEventStorage>(), new ResolvedParameter<IEventBus>(), new ResolvedParameter<ISnapshotProvider>()));
        }

        public static void AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("EventSourcedRepository_StorageMappings.xml"))
                .RegisterType<IStorage, SqlStorage>("eventStore", new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IStorage, SqlStorage>("snapshotStore", new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(Helper.CreateMessageQueueName(EventBus_MessageQueue), true))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<ISnapshotProvider, EventNumberSnapshotProvider>(new InjectionConstructor(new ResolvedParameter<IStorage>("eventStore"), new ResolvedParameter<IStorage>("snapshotStore"), new InjectionParameter<SnapshotProviderOption>(SnapshotProviderOption.Immediate), new InjectionParameter<int>(5)))
                .RegisterType<IDomainRepository, EventSourcedDomainRepository>(new InjectionConstructor(new ResolvedParameter<IDomainEventStorage>(), new ResolvedParameter<IEventBus>(), new ResolvedParameter<ISnapshotProvider>()));
        }

        public static void AppInit_Buses_EventSourcedDomainRepositoryWithDirectCommandBusButWithoutSnapshotProvider(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>("DomainEventStorageMappingResolver", new InjectionConstructor("StorageMappings.xml"))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(CQRSTestDB_SQLExpress_ConnectionString,  new ResolvedParameter<IStorageMappingResolver>("DomainEventStorageMappingResolver")))
                .RegisterType<IMessageDispatcher, MessageDispatcher>(new ContainerControlledLifetimeManager())
                .RegisterType<ICommandBus, DirectCommandBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IMessageDispatcher>()))
                .RegisterType<IEventBus, DirectEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IMessageDispatcher>()))
                .RegisterType<ISnapshotProvider, SuppressedSnapshotProvider>()
                .RegisterType<IDomainRepository, EventSourcedDomainRepository>(new InjectionConstructor(new ResolvedParameter<IDomainEventStorage>(), new ResolvedParameter<IEventBus>(), new ResolvedParameter<ISnapshotProvider>()));
        }

        public static void AppInit_MessageDispatcher(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IMessageDispatcher, Apworks.Bus.MessageDispatcher>();
        }

        public static void AppInit_EFRepository(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<DbContext, EFTestContext>()
                .RegisterType<IRepositoryContext, EntityFrameworkRepositoryContext>()
                .RegisterType<IRepository<EFCustomer>, EntityFrameworkRepository<EFCustomer>>()
                .RegisterType<IRepository<EFCustomerNote>, EntityFrameworkRepository<EFCustomerNote>>();
        }

        public static void AppInit_MongoDBRepository(object sender, AppInitEventArgs e)
        {
            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            c.RegisterType<IMongoDBRepositoryContextSettings, MongoDBRepositoryContextSettings>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryContext, MongoDBRepositoryContext>()
                .RegisterType<IRepository<Customer>, MongoDBRepository<Customer>>();
            MongoDBRepositoryContext.RegisterConventions();
        }
        #endregion
    }
}
