using System;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Tests.Buses.Commands;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.Buses
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CommandBusTests
    {
        private static IApp application;
        public CommandBusTests()
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
            IConfigSource configSource = Helper.ConfigSource_Buses_EventSourcedDomainRepositoryWithDirectCommandBusButWithoutSnapshotProvider;
            application = AppRuntime.Create(configSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(Helper.AppInit_Buses_EventSourcedDomainRepositoryWithDirectCommandBusButWithoutSnapshotProvider);
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
        public void Buses_CommandBus_ChangeEmailFor1005TimesTest()
        {
            SourcedCustomer customer = new SourcedCustomer();
            using (IDomainRepository domainRepository = application.ObjectContainer.GetService<IDomainRepository>())
            {
                domainRepository.Save(customer);
                domainRepository.Commit();
            }

            ChangeEmailCommand cmd = new ChangeEmailCommand(customer.ID, "my@163.com");
            using (ICommandBus bus = application.ObjectContainer.GetService<ICommandBus>())
            {
                bus.Publish(cmd);
                bus.Commit();
            }
        }
    }
}
