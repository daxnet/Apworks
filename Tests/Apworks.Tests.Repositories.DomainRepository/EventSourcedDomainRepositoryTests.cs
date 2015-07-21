using System;
using System.Data;
using Apworks.Application;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Apworks.Tests.Repositories.DomainRepository
{
    /// <summary>
    /// Summary description for EventSourcedDomainRepositoryTests
    /// </summary>
    [TestClass]
    public class EventSourcedDomainRepositoryTests
    {
        public EventSourcedDomainRepositoryTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Helper.ClearCQRSSQLExpressTestDB();
            Helper.ClearMessageQueue(Helper.EventBus_MessageQueue);
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_SaveAggregateRootTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            app.Start();

            SourcedCustomer customer = new SourcedCustomer();
            customer.ChangeName("sunny", "chen");
            Assert.AreEqual<long>(2, customer.Version);
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            Assert.AreEqual<long>(2, customer.Version);
            int recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_DomainEvents);
            Assert.AreEqual<int>(2, recordCnt);
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(2, msgCnt);
        }

        [TestMethod]
        public async Task EventSourcedDomainRepositoryTests_SaveAggregateRootTestAsync()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            app.Start();

            SourcedCustomer customer = new SourcedCustomer();
            customer.ChangeName("sunny", "chen");
            Assert.AreEqual<long>(2, customer.Version);
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                await domainRepository.CommitAsync();
            }
            Assert.AreEqual<long>(2, customer.Version);
            int recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_DomainEvents);
            Assert.AreEqual<int>(2, recordCnt);
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(2, msgCnt);
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_SaveAndLoadAggregateRootTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusButWithoutSnapshotProvider;
            app.Start();

            SourcedCustomer customer = new SourcedCustomer();
            customer.ChangeName("sunny", "chen");
            customer.ChangeEmail("acqy@163.com");
            Assert.AreEqual<long>(3, customer.Version);
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            Assert.AreEqual<long>(3, customer.Version);

            using (IDomainRepository domainRepository2 = app.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer cust = domainRepository2.Get<SourcedCustomer>(customer.ID);
                Assert.AreEqual<long>(3, cust.Version);
                Assert.AreEqual<string>("sunny", cust.FirstName);
                Assert.AreEqual<string>("chen", cust.LastName);
                Assert.AreEqual<string>("acqy@163.com", cust.Email);
            }
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_CreateSnapshotTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();

                SourcedCustomer cust2 = domainRepository.Get<SourcedCustomer>(customer.ID);
                Assert.AreEqual<long>(5, cust2.Version);
                Assert.AreEqual<string>("acqy3@163.com", cust2.Email);

                cust2.ChangeName("sunny", "chen");
                domainRepository.Save(cust2);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_ValidateSnapshotVersion1()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            // first to produce 5 events
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // produce another event to trigger snapshot creation
            customer.ChangeName("qingyang", "chen");
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(6, Convert.ToInt64(dr["Version"]));
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_ValidateSnapshotVersion2()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            // first to produce 5 events
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // produce another 5 events to trigger snapshot creation
            for (int i = 0; i < 5; i++)
            {
                customer.ChangeName("qingyang" + i.ToString(), "chen");
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(10, Convert.ToInt64(dr["Version"]));
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_ValidateSnapshotVersion3()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            // first to produce 5 events
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // produce another 5 events to trigger snapshot creation
            for (int i = 0; i < 5; i++)
            {
                customer.ChangeName("qingyang" + i.ToString(), "chen");
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(10, Convert.ToInt64(dr["Version"]));
            // produce another 3 events, the snapshot should remains the same...
            for (int i = 0; i < 3; i++)
            {
                customer.ChangeName("qingyang", "chen" + i.ToString());
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(10, Convert.ToInt64(dr["Version"]));
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_UpdateSnapshotTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            // first to produce 5 events
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // produce another 5 events to trigger snapshot creation
            for (int i = 0; i < 5; i++)
            {
                customer.ChangeName("qingyang" + i.ToString(), "chen");
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(10, Convert.ToInt64(dr["Version"]));

            // produce another 6 events, the snapshot should be updated...
            for (int i = 0; i < 6; i++)
            {
                customer.ChangeName("qingyang", "chen" + i.ToString());
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(16, Convert.ToInt64(dr["Version"]));
        }

        [TestMethod]
        public void EventSourcedDomainRepositoryTests_GetFromSnapshotTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithMSMQBusAndSnapshotProvider;
            app.Start();
            SourcedCustomer customer = new SourcedCustomer();
            for (int i = 0; i < 4; i++)
            {
                customer.ChangeEmail("acqy" + i.ToString() + "@163.com");
            }
            Assert.AreEqual<long>(5, customer.Version);
            // first to produce 5 events
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // produce another 1 event to trigger snapshot creation
            customer.ChangeName("qingyang111", "chen");
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(6, Convert.ToInt64(dr["Version"]));
            // produce another 3 events
            for (int i = 0; i < 3; i++)
            {
                customer.ChangeName("sunny" + i.ToString(), "chen");
            }
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            // the snapshot won't be updated...
            snapshotCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, snapshotCnt);
            dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            dr = dt.Rows[0];
            Assert.AreEqual<string>(typeof(SourcedCustomer).AssemblyQualifiedName, dr["AggregateRootType"].ToString());
            Assert.AreEqual<Guid>(customer.ID, (Guid)(dr["AggregateRootID"]));
            Assert.AreEqual<long>(6, Convert.ToInt64(dr["Version"]));

            int eventCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_DomainEvents);
            Assert.AreEqual<int>(9, eventCnt);

            // now retrieve
            using (IDomainRepository domainRepository = app.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer cust2 = domainRepository.Get<SourcedCustomer>(customer.ID);
                Assert.AreEqual<long>(9, cust2.Version);
                Assert.AreEqual<string>("sunny2", cust2.FirstName);
                Assert.AreEqual<string>("acqy3@163.com", cust2.Email);
            }
        }
    }
}
