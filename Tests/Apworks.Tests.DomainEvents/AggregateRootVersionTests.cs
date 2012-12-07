using System;
using System.Collections.Generic;
using System.Linq;
using Apworks.Application;
using Apworks.Config;
using Apworks.Events;
using Apworks.Repositories;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Apworks.Tests.Common.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.DomainEvents
{
    /// <summary>
    /// Summary description for AggregateRootVersionTests
    /// </summary>
    [TestClass]
    public class AggregateRootVersionTests
    {
        private static IApp application;
        public AggregateRootVersionTests()
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
            IConfigSource configSource = Helper.ConfigSource_AggregateRootVersion;
            application = AppRuntime.Create(configSource);
            application.Initialize += new System.EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_EventSourcedDomainRepositoryWithDirectEventBusButWithoutSnapshotProvider);
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
        public void AggregateRootVersionTests_SimpleVersionTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            customer.ChangeName("Qingyang", "Chen");
            customer.ChangeEmail("acqy@163.com");
            Assert.AreEqual<long>(3, customer.Version);
        }

        [TestMethod]
        public void AggregateRootVersionTests_BuildFromHistoryTest()
        {
            var aggregateRootId = Guid.NewGuid();
            List<IDomainEvent> historicalEvents = new List<IDomainEvent>();
            historicalEvents.Add(new CreateCustomerDomainEvent
            {
                SourceID = aggregateRootId,
                AssemblyQualifiedSourceType = typeof(SourcedCustomer).AssemblyQualifiedName,
                Branch = 0,
                FirstName = "Sunny",
                ID = Helper.AggregateRootId1,
                LastName = "Chen",
                Timestamp = DateTime.UtcNow,
                Username = "daxnet",
                Version = 1
            });
            historicalEvents.Add(new ChangeCustomerNameDomainEvent
            {
                SourceID = aggregateRootId,
                AssemblyQualifiedSourceType = typeof(SourcedCustomer).AssemblyQualifiedName,
                Branch = 0,
                FirstName = "Qingyang",
                ID = Helper.AggregateRootId2,
                LastName = "Chen",
                Timestamp = DateTime.UtcNow,
                Version = 2
            });
            historicalEvents.Add(new ChangeEmailDomainEvent
            {
                SourceID = aggregateRootId,
                AssemblyQualifiedSourceType = typeof(SourcedCustomer).AssemblyQualifiedName,
                Branch = 0,
                ID = Helper.AggregateRootId3,
                Timestamp = DateTime.UtcNow,
                Email = "acqy@163.com",
                Version = 3
            });
            SourcedCustomer customer = new SourcedCustomer();
            Assert.AreEqual<long>(1, customer.Version);
            customer.BuildFromHistory(historicalEvents);
            Assert.AreEqual<long>(3, customer.Version);
        }

        [TestMethod]
        public void AggregateRootVersionTests_AlongWithEventSourcedDomainRepositoryTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            Guid id = customer.ID;
            customer.ChangeName("Qingyang", "Chen");
            customer.ChangeEmail("acqy@163.com");
            Assert.AreEqual<long>(3, customer.Version);
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save<SourcedCustomer>(customer);
                domainRepository.Commit();
            }
            Assert.AreEqual<long>(3, customer.Version);
            Assert.AreEqual<int>(0, customer.UncommittedEvents.Count());

            SourcedCustomer customer2 = null;
            using (IDomainRepository domainRepository2 = application.ObjectContainer.GetService<IDomainRepository>())
            {
                customer2 = domainRepository2.Get<SourcedCustomer>(id);
                Assert.AreEqual<long>(3, customer2.Version);
                customer2.ChangeName("Joe", "Li");
                customer2.ChangeEmail("jli@hp.com");
                Assert.AreEqual<long>(5, customer2.Version);
                Assert.AreEqual<int>(2, customer2.UncommittedEvents.Count());
                domainRepository2.Save<SourcedCustomer>(customer2);
                domainRepository2.Commit();
                Assert.AreEqual<long>(5, customer2.Version);
                Assert.AreEqual<int>(0, customer2.UncommittedEvents.Count());
            }

            SourcedCustomer customer3 = null;
            using (IDomainRepository domainRepository3 = application.ObjectContainer.GetService<IDomainRepository>())
            {
                customer3 = domainRepository3.Get<SourcedCustomer>(id);
                Assert.AreEqual<long>(5, customer3.Version);
                Assert.AreEqual<int>(0, customer3.UncommittedEvents.Count());
            }
        }
    }
}
