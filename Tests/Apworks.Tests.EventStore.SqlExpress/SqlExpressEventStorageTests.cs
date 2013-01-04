using System.Data.SqlClient;
using System.Linq;
using Apworks.Application;
using Apworks.Config;
using Apworks.Events.Storage;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.EventStore.SqlExpress
{
    
    /// <summary>
    /// Summary description for DomainEventStoreTests
    /// </summary>
    [TestClass]
    public class DomainEventStoreTests
    {
        private static IApp application;

        public DomainEventStoreTests()
        {
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
            IConfigSource configSource = Helper.ConfigSource_EventStore_SqlExpress;
            application = AppRuntime.Create(configSource);
            application.Initialize += new System.EventHandler<AppInitEventArgs>(Helper.AppInit_EventStore_SqlExpress);
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
            using (SqlConnection c = new SqlConnection(Helper.CQRSTestDB_SQLExpress_ConnectionString))
            {
                c.Open();
                using (SqlCommand command = new SqlCommand("delete from DomainEvents", c))
                {
                    command.ExecuteNonQuery();
                }
                c.Close();
            }
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [Description("Test the adding of a domain event to the store.")]
        public void EventStore_SqlExpress_AddDomainEventToStoreTest()
        {
            using (IDomainEventStorage eventStore = application.ObjectContainer.GetService<IDomainEventStorage>())
            {
                var domainEvent = Helper.CreateCreateCustomerDomainEvents()[0];
                eventStore.SaveEvent(domainEvent);
                eventStore.Commit();
            }
        }

        [TestMethod]
        [Description("Test the loading of all domain events from the store.")]
        public void EventStore_SqlExpress_RetrieveAllDomainEventFromStoreTest()
        {
            using (IDomainEventStorage eventStore = application.ObjectContainer.GetService<IDomainEventStorage>())
            {
                foreach (var cce in Helper.CreateCreateCustomerDomainEvents())
                {
                    eventStore.SaveEvent(cce);
                }
                //eventStore.Commit();
            }
            using (IDomainEventStorage eventStore = application.ObjectContainer.GetService<IDomainEventStorage>())
            {
                var p = eventStore.LoadEvents(typeof(SourcedCustomer), Helper.AggregateRootId1);
                var q = eventStore.LoadEvents(typeof(SourcedCustomer), Helper.AggregateRootId2);
                var r = eventStore.LoadEvents(typeof(SourcedCustomer), Helper.AggregateRootId3);
                Assert.IsNotNull(p);
                Assert.IsNotNull(q);
                Assert.IsNotNull(r);
                Assert.AreEqual<int>(1, p.Count());
                Assert.AreEqual<int>(1, q.Count());
                Assert.AreEqual<int>(1, r.Count());
            }
        }
    }
}
