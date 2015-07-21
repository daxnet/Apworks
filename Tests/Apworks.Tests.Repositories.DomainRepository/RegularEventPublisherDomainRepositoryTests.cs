using System;
using System.Linq;
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
    /// Summary description for RegularEventPublisherDomainRepositoryTests
    /// </summary>
    [TestClass]
    public class RegularEventPublisherDomainRepositoryTests
    {
        private static IApp application;

        public RegularEventPublisherDomainRepositoryTests()
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
            IConfigSource configSource = Helper.ConfigSource_Repositories_RegularEventPublisherDomainRepository_MSMQ;
            application = AppRuntime.Create(configSource);
            application.Initialize+=new EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_RegularEventPublisherDomainRepository_MSMQ);
            application.Start();
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
        public void RegularEventPublisherDomainRepositoryTests_SaveAggregateRootTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            customer.ChangeEmail("acqy@163.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(3, msgCnt);
            int recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            Assert.AreEqual<int>(1, recordCnt);
        }

        [TestMethod]
        public async Task RegularEventPublisherDomainRepositoryTests_SaveAggregateRootTestAsync()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            customer.ChangeEmail("acqy@163.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                await domainRepository.CommitAsync();
            }
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(3, msgCnt);
            int recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            Assert.AreEqual<int>(1, recordCnt);
        }

        [TestMethod]
        public void RegularEventPublisherDomainRepositoryTests_SaveAndLoadAggregateRootTest()
        {
            SourcedCustomer cust1 = new SourcedCustomer();
            Guid id1=cust1.ID;
            cust1.ChangeName("Qingyang", "Chen");
            cust1.ChangeEmail("acqy@163.com");
            SourcedCustomer cust2 = new SourcedCustomer();
            Guid id2 = cust2.ID;
            cust2.ChangeName("Jim", "Liu");
            cust2.ChangeEmail("jl@163.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save(cust1);
                domainRepository.Save(cust2);
                domainRepository.Commit();
            }
            int msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(6, msgCnt);
            int recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            Assert.AreEqual<int>(2, recordCnt);
            SourcedCustomer cust3 = new SourcedCustomer();
            using (IDomainRepository domainRepository2 = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository2.Save(cust3);
                SourcedCustomer cust4 = domainRepository2.Get<SourcedCustomer>(id1);
                int c = cust4.UncommittedEvents.Count();
                if (c > 0)
                {
                    var evt = cust4.UncommittedEvents.FirstOrDefault();
                }
                cust4.ChangeEmail("daxnet@live.com");
                domainRepository2.Save(cust4);
                domainRepository2.Commit();
            }
            msgCnt = Helper.GetMessageQueueCount(Helper.EventBus_MessageQueue);
            Assert.AreEqual<int>(8, msgCnt);
            recordCnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            Assert.AreEqual<int>(3, recordCnt);
        }
    }
}
