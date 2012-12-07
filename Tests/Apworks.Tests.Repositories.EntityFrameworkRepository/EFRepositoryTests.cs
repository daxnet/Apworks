using System;
using System.Data.Entity;
using Apworks.Application;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Apworks.Tests.Common.EFContexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.Repositories.EntityFrameworkRepository
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EFRepositoryTests
    {
        private static IApp application;
        public EFRepositoryTests()
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
            Database.SetInitializer<EFTestContext>(new DropCreateDatabaseIfModelChanges<EFTestContext>());
            IConfigSource configSource = Helper.ConfigSource_EFRepository;
            application = AppRuntime.Create(configSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(Helper.AppInit_EFRepository);
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
            Helper.ClearEFTestDB();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void EntityFrameworkRepositoryTests_SaveAggregateRootTest()
        {
            EFCustomer customer = new EFCustomer
            {
                Address = new EFAddress("China", "SH", "SH", "A street", "12345"),
                UserName = "daxnet",
                Password = "123456"
            };
            
            IRepository<EFCustomer> customerRepository = ServiceLocator.Instance.GetService<IRepository<EFCustomer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            customerRepository.Context.Dispose();

            int expected = 1;
            int actual = Helper.ReadRecordCountFromEFTestDB(Helper.EF_Table_EFCustomers);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EntityFrameworkRepositoryTests_SaveAggregateRootWithIDReturnedTest()
        {
            EFCustomer customer = new EFCustomer
            {
                Address = new EFAddress("China", "SH", "SH", "A street", "12345"),
                UserName = "daxnet",
                Password = "123456"
            };

            IRepository<EFCustomer> customerRepository = ServiceLocator.Instance.GetService<IRepository<EFCustomer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            customerRepository.Context.Dispose();

            Assert.AreNotEqual(0, customer.ID);
        }

        [TestMethod]
        public void EntityFrameworkRepositoryTests_SaveAndLoadAggregateRootTest()
        {
            EFCustomer customer = new EFCustomer
            {
                Address = new EFAddress("China", "SH", "SH", "A street", "12345"),
                UserName = "daxnet",
                Password = "123456"
            };
            IRepository<EFCustomer> customerRepository = ServiceLocator.Instance.GetService<IRepository<EFCustomer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            var key = customer.ID;
            EFCustomer customer2 = customerRepository.GetByKey(key);
            customerRepository.Context.Dispose();
            
            Assert.AreEqual(customer.UserName, customer2.UserName);
            Assert.AreEqual(customer.Password, customer2.Password);
            Assert.AreEqual(customer.Address.City, customer2.Address.City);
            Assert.AreEqual(customer.Address.Country, customer2.Address.Country);
            Assert.AreEqual(customer.Address.State, customer2.Address.State);
            Assert.AreEqual(customer.Address.Street, customer2.Address.Street);
            Assert.AreEqual(customer.Address.Zip, customer2.Address.Zip);
        }

        [TestMethod]
        public void EntityFrameworkRepositoryTests_SaveAggregateTest()
        {

        }
    }
}
