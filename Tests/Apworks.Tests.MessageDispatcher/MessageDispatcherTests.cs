using System;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Config;
using Apworks.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.MessageDispatcher
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MessageDispatcherTests
    {
        private static IApp application;

        public MessageDispatcherTests()
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
            IConfigSource configSource = Helper.ConfigSource_MessageDispatcher;
            application = AppRuntime.Create(configSource);
            application.Initialize += Helper.AppInit_MessageDispatcher;
            application.Start();
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MessageDispatcher_CreateTest()
        {
            Apworks.Bus.IMessageDispatcher messageDispatcher = Apworks.Bus.MessageDispatcher.CreateAndRegister(application.ConfigSource, typeof(Apworks.Bus.MessageDispatcher));
            Assert.IsNotNull(messageDispatcher);
        }

        [TestMethod]
        public void MessageDispatcher_DispatchCustomerCreatedEventTest()
        {
            Apworks.Bus.IMessageDispatcher messageDispatcher = Apworks.Bus.MessageDispatcher.CreateAndRegister(application.ConfigSource, typeof(Apworks.Bus.MessageDispatcher));
            MessageDispatchEventArgs evtArgs = null;
            messageDispatcher.Dispatched += (s, e) =>
                {
                    evtArgs = e;
                };
            var message = new Apworks.Tests.Common.Events.CreateCustomerDomainEvent();
            messageDispatcher.DispatchMessage(message);
            Assert.IsNotNull(evtArgs);
            Assert.AreEqual<Type>(typeof(Apworks.Tests.Common.MessageHandlers.CustomerCreatedEventHandler), evtArgs.HandlerType);
        }

        [TestMethod]
        public void MessageDispatcher_DispachEmailChangedEventTest()
        {
            Apworks.Bus.IMessageDispatcher messageDispatcher = Apworks.Bus.MessageDispatcher.CreateAndRegister(application.ConfigSource, typeof(Apworks.Bus.MessageDispatcher));
            MessageDispatchEventArgs evtFail = null;
            MessageDispatchEventArgs evtSuccess = null;
            messageDispatcher.DispatchFailed += (s, e) =>
                {
                    evtFail = e;
                };
            messageDispatcher.Dispatched += (s, e) =>
                {
                    evtSuccess = e;
                };
            var message = new Apworks.Tests.Common.Events.ChangeEmailDomainEvent();
            messageDispatcher.DispatchMessage(message);
            Assert.IsNotNull(evtFail);
            Assert.IsNotNull(evtSuccess);
            Assert.AreEqual<Type>(typeof(Apworks.Tests.Common.MessageHandlers.YetAnotherEmailChangedEventHandler), evtFail.HandlerType);
            Assert.AreEqual<Type>(typeof(Apworks.Tests.Common.MessageHandlers.EmailChangedEventHandler), evtSuccess.HandlerType);
        }
    }
}
