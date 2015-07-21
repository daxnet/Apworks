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
    /// Summary description for RegularDomainRepositoryTests
    /// </summary>
    [TestClass]
    public class RegularDomainRepositoryTests
    {
        private static IApp application;

        public RegularDomainRepositoryTests()
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
            IConfigSource configSource = Helper.ConfigSource_Repositories_RegularDomainRepository;
            application = AppRuntime.Create(configSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_RegularDomainRepository);
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
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void RegularDomainRepositoryTests_SaveAggregateRootTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeEmail("daxnet@live.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();

                int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
                Assert.AreEqual<int>(1, cnt);
                SourcedCustomer customer2 = null;

                customer2 = domainRepository.Get<SourcedCustomer>(id);
                Assert.IsNotNull(customer2);
                Assert.AreEqual<string>("Sunny", customer2.FirstName);
                Assert.AreEqual<string>("Chen", customer2.LastName);
                Assert.AreEqual<string>("daxnet@live.com", customer2.Email);
            }
        }

        [TestMethod]
        public async Task RegularDomainRepositoryTests_SaveAggregateRootTestAsync()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeEmail("daxnet@live.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                await domainRepository.CommitAsync();

                int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
                Assert.AreEqual<int>(1, cnt);
                SourcedCustomer customer2 = null;

                customer2 = domainRepository.Get<SourcedCustomer>(id);
                Assert.IsNotNull(customer2);
                Assert.AreEqual<string>("Sunny", customer2.FirstName);
                Assert.AreEqual<string>("Chen", customer2.LastName);
                Assert.AreEqual<string>("daxnet@live.com", customer2.Email);
            }
        }

        [TestMethod]
        public void RegularDomainRepositoryTests_UpdateAggregateRootTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeEmail("daxnet@live.com");
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();

                int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
                Assert.AreEqual<int>(1, cnt);
                customer.ChangeEmail("acqy@163.com");

                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();

                int cnt2 = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
                Assert.AreEqual<int>(1, cnt2);
                DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
                DataRow dr = dt.Rows[0];
                Assert.AreEqual<string>("acqy@163.com", dr["Email"].ToString());
            }
        }

        [TestMethod]
        public void RegularDomainRepositoryTests_GetAggregateRootTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }

            using (IDomainRepository domainRepository2 = application.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer customer2 = domainRepository2.Get<SourcedCustomer>(id);
                Assert.AreEqual<string>("Sunny", customer2.FirstName);
                Assert.AreEqual<string>("Chen", customer2.LastName);
                Assert.AreEqual<string>("daxnet", customer2.Username);
            }
        }

        [TestMethod]
        public void RegularDomainRepositoryTests_GetAndSaveAggregateRootTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }

            using (IDomainRepository domainRepository2 = application.ObjectContainer.GetService<IDomainRepository>())
            {
                SourcedCustomer customer2 = domainRepository2.Get<SourcedCustomer>(id);
                customer2.ChangeEmail("acqy@163.com");
                domainRepository2.Save(customer2);
                domainRepository2.Commit();
            }

            int cnt = Helper.ReadRecordCountFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            Assert.AreEqual<int>(1, cnt);
            DataTable dt = Helper.ReadRecordsFromSQLExpressCQRSTestDB(Helper.CQRSTestDB_Table_SourcedCustomer);
            DataRow dr = dt.Rows[0];
            Assert.AreEqual<string>("acqy@163.com", dr["Email"].ToString());
        }
    }
}
