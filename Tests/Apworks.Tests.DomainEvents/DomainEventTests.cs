using System.Linq;
using Apworks.Application;
using Apworks.Config;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Apworks.Tests.Common.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Apworks.Events;

namespace Apworks.Tests.DomainEvents
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DomainEventTests
    {
        private static IApp application;
        public DomainEventTests()
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
            IConfigSource configSource = Helper.ConfigSource_DomainEvents;
            application = AppRuntime.Create(configSource);
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
            DomainEvent.UnsubscribeAll();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [Description("Test applying the domain event when constructing the aggregate root.")]
        public void DomainEvents_ApplyDomainEventOnConstructionTest()
        {
            SourcedCustomer sourcedCustomer = new SourcedCustomer();
            Assert.AreEqual<string>("daxnet", sourcedCustomer.Username);
            Assert.AreEqual<int>(1, sourcedCustomer.UncommittedEvents.Count());
        }

        [TestMethod]
        [Description("Test applying the domain event.")]
        public void DomainEvents_ApplyDomainEventTest()
        {
            SourcedCustomer sourcedCustomer = new SourcedCustomer();
            Assert.AreEqual<string>("daxnet", sourcedCustomer.Username);
            Assert.AreEqual<string>("Sunny", sourcedCustomer.FirstName);
            Assert.AreEqual<string>("Chen", sourcedCustomer.LastName);
            sourcedCustomer.ChangeName("sunny", "chen");
            Assert.AreEqual<string>("sunny", sourcedCustomer.FirstName);
            Assert.AreEqual<string>("chen", sourcedCustomer.LastName);
            Assert.AreEqual<int>(2, sourcedCustomer.UncommittedEvents.Count());
        }

        [TestMethod]
        [Description("Test creating of the sourced aggregate root.")]
        public void DomainEvents_CreateSourcedAggregateRootTest()
        {
            SourcedCustomer sourcedCustomer = new SourcedCustomer();
            Assert.AreNotEqual<Guid>(Guid.Empty, sourcedCustomer.ID);
            Assert.AreEqual<int>(1, sourcedCustomer.UncommittedEvents.Count());
            var uncommittedEvent = sourcedCustomer.UncommittedEvents.First();
            Assert.IsInstanceOfType(uncommittedEvent, typeof(CreateCustomerDomainEvent));
        }

        [TestMethod]
        public void DomainEvents_SubscribeDomainEventHandlerTest()
        {
            DomainEvent.Subscribe<OneDomainEvent>(new OneDomainEventHandler());
            DomainEvent.Subscribe<AnotherDomainEvent>(new AnotherDomainEventHandler());
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            var s2 = DomainEvent.GetSubscriptions<AnotherDomainEvent>().Count();
            Assert.AreEqual(1, s1);
            Assert.AreEqual(1, s2);
        }

        [TestMethod]
        public void DomainEvents_SubscribeSameDomainEventHandlerTest()
        {
            var a = new OneDomainEventHandler();
            DomainEvent.Subscribe<OneDomainEvent>(a);
            DomainEvent.Subscribe<OneDomainEvent>(a);
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(1, s1);
        }

        [TestMethod]
        public void DomainEvents_SubscribeTwoDomainEventHandlerTest()
        {
            DomainEvent.Subscribe<OneDomainEvent>(new OneDomainEventHandler());
            DomainEvent.Subscribe<OneDomainEvent>(new OneDomainEventAnotherHandler());
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(2, s1);
        }

        [TestMethod]
        public void DomainEvents_UnsubscribeDomainEventHandlerTest()
        {
            var a = new OneDomainEventHandler();
            var b = new OneDomainEventAnotherHandler();
            DomainEvent.Subscribe<OneDomainEvent>(a);
            DomainEvent.Subscribe<OneDomainEvent>(b);
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(2, s1);
            DomainEvent.Unsubscribe<OneDomainEvent>(a);
            var s2 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(1, s2);
        }

        [TestMethod]
        public void DomainEvents_SubscribeFuncHandlerTest()
        {
            Func<OneDomainEvent, bool> a = p => true;
            DomainEvent.Subscribe<OneDomainEvent>(a);
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(1, s1);
        }

        [TestMethod]
        public void DomainEvents_SubscribeFuncHandlerTwiceTest()
        {
            Func<OneDomainEvent, bool> a = p => true;
            DomainEvent.Subscribe<OneDomainEvent>(a);
            DomainEvent.Subscribe<OneDomainEvent>(a);
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(1, s1);
        }

        [TestMethod]
        public void DomainEvents_UnsubscribeFuncHandlerTest()
        {
            Func<OneDomainEvent, bool> a = p => true;
            DomainEvent.Subscribe<OneDomainEvent>(a);
            var s1 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(1, s1);
            DomainEvent.Unsubscribe<OneDomainEvent>(a);
            var s2 = DomainEvent.GetSubscriptions<OneDomainEvent>().Count();
            Assert.AreEqual(0, s2);
        }
    }
}
