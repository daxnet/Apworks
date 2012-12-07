using System.Linq;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Config;
using Apworks.Tests.Common;
using Apworks.Tests.Common.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;

namespace Apworks.Tests.Interception
{
    /// <summary>
    /// Summary description for InitializeInterceptorsTests
    /// </summary>
    [TestClass]
    public class InitializeInterceptorsTests
    {
        public InitializeInterceptorsTests()
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
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void InitializeInterceptorsTests_InitAppTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("exception", typeof(ExceptionHandlingInterceptor));
            configSource.AddInterceptor("logging", typeof(LoggingInterceptor));
            IApp app = AppRuntime.Create(configSource);
            Assert.IsNotNull(app.Interceptors);
            Assert.AreEqual<int>(2, app.Interceptors.Count());
            Assert.IsInstanceOfType(app.Interceptors.First(), typeof(ExceptionHandlingInterceptor));
            Assert.IsInstanceOfType(app.Interceptors.Last(), typeof(LoggingInterceptor));
        }

        [TestMethod]
        public void InitializeInterceptorsTests_InitAppAndHitAgainTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("exception", typeof(ExceptionHandlingInterceptor));
            configSource.AddInterceptor("logging", typeof(LoggingInterceptor));
            IApp app = AppRuntime.Create(configSource);
            Assert.IsNotNull(app.Interceptors);
            Assert.AreEqual<int>(2, app.Interceptors.Count());
            Assert.IsInstanceOfType(app.Interceptors.First(), typeof(ExceptionHandlingInterceptor));
            Assert.IsInstanceOfType(app.Interceptors.Last(), typeof(LoggingInterceptor));
            var interceptors = AppRuntime.Instance.CurrentApplication.Interceptors;
            Assert.IsNotNull(interceptors);
        }

        [TestMethod]
        public void InitializeInterceptorsTests_MockMultipleInterceptorTest()
        {
            Helper.ClearApp(AppRuntime.Instance);
            bool a = false;
            bool b = false;
            MockInterceptorA.InterceptOccur += (s, e) =>
            {
                a = true;
            };
            MockInterceptorB.InterceptOccur += (s, e) =>
            {
                b = true;
            };

            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("a", typeof(MockInterceptorA));
            configSource.AddInterceptor("b", typeof(MockInterceptorB));
            configSource.AddInterceptorRef(typeof(MessageDispatcher), typeof(MessageDispatcher).GetMethod("Clear", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), "a");
            configSource.AddInterceptorRef(typeof(MessageDispatcher), typeof(MessageDispatcher).GetMethod("Clear", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), "b");

            IApp app = AppRuntime.Create(configSource);
            app.Initialize += (s, e) =>
            {
                UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
                c.RegisterType<IMessageDispatcher, MessageDispatcher>();
            };
            app.Start();
            IMessageDispatcher dispatcher = app.ObjectContainer.GetService<IMessageDispatcher>();
            dispatcher.Clear();
            Assert.IsTrue(a);
            Assert.IsTrue(b);

        }

        [TestMethod]
        public void InitializeInterceptorsTests_MockSingleInterceptorTest()
        {
            Helper.ClearApp(AppRuntime.Instance);
            bool a = false;
            bool b = false;
            MockInterceptorA.InterceptOccur += (s, e) =>
            {
                a = true;
            };
            MockInterceptorB.InterceptOccur += (s, e) =>
            {
                b = true;
            };

            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("a", typeof(MockInterceptorA));
            configSource.AddInterceptor("b", typeof(MockInterceptorB));
            configSource.AddInterceptorRef(typeof(MessageDispatcher), typeof(MessageDispatcher).GetMethod("Clear", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), "a");
            IApp app = AppRuntime.Create(configSource);
            app.Initialize += (s, e) =>
            {
                UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
                c.RegisterType<IMessageDispatcher, MessageDispatcher>();
            };
            app.Start();
            IMessageDispatcher dispatcher = app.ObjectContainer.GetService<IMessageDispatcher>();
            dispatcher.Clear();
            Assert.IsTrue(a);
            Assert.IsFalse(b);
        }
    }
}
