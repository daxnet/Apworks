using Apworks.Application;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Apworks.Tests.Repositories.DomainRepository
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SnapshotDomainRepositoryTests
    {
        public SnapshotDomainRepositoryTests()
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            
        }
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
        public void SnapshotDomainRepositoryTests_SaveAggregateRootAndPublishToDirectBusTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_SnapshotDomainRepository_DirectBus;
            IApp application = AppRuntime.Create(configSource);
            application.Initialize += new System.EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_SnapshotDomainRepository_DirectBus);
            application.Start();

            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, cnt);
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer sourcedCustomer = null;
                sourcedCustomer = domainRepository.Get<SourcedCustomer>(id);
                Assert.AreEqual<string>("Qingyang", sourcedCustomer.FirstName);
                Assert.AreEqual<string>("Chen", sourcedCustomer.LastName);
            }
        }

        [TestMethod]
        public void SnapshotDomainRepositoryTests_SaveAggregateRootAndPublishToMSMQTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_SnapshotDomainRepository_MSMQ;
            IApp application = AppRuntime.Create(configSource);
            application.Initialize += new System.EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_SnapshotDomainRepository_MSMQ);
            application.Start();

            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(1, cnt);
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer sourcedCustomer = null;
                sourcedCustomer = domainRepository.Get<SourcedCustomer>(id);
                Assert.AreEqual<string>("Qingyang", sourcedCustomer.FirstName);
                Assert.AreEqual<string>("Chen", sourcedCustomer.LastName);
            }
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(2, msgCnt);
        }

        [TestMethod]
        public void SnapshotDomainRepositoryTests_SaveAggregateRootButFailPublishToMSMQTest()
        {
            IConfigSource configSource = Helper.ConfigSource_Repositories_SnapshotDomainRepository_SaveButFailPubToMSMQ;
            IApp application = AppRuntime.Create(configSource);
            application.Initialize += new System.EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_SnapshotDomainRepository_SaveButFailPubToMSMQ);
            application.Start();

            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            IDomainRepository domainRepository = null;
            try
            {
                using (domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
                {
                    domainRepository.Save<SourcedCustomer>(customer);
                    domainRepository.Commit();
                }
            }
            catch { }
            int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_Snapshots);
            Assert.AreEqual<int>(0, cnt);
        }
    }
}
