using System;
using System.Linq;
using Apworks.Application;
using Apworks.Config;
using Apworks.Exceptions;
using Apworks.Storage;
using Apworks.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.ExceptionHandling
{
    /// <summary>
    /// Summary description for ExceptionHandlingTests
    /// </summary>
    [TestClass]
    public class ExceptionHandlingTests
    {
        private static IApp application;
        public ExceptionHandlingTests()
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
            IConfigSource configSource = Helper.ConfigSource_ExceptionHandling;
            application = AppRuntime.Create(configSource);
            application.Initialize += Helper.AppInit_ExceptionHandling_InvalidStorage;
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
            Helper.DeleteTempFile();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForInvalidOperationExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<InvalidOperationException>();
            Assert.AreEqual<int>(1, handlers.Count());
            Assert.IsInstanceOfType(handlers.First(), typeof(Apworks.Tests.Common.ExceptionHandlers.InvalidOperationExceptionHandler));
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForArgumentNullExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<ArgumentNullException>();
            Assert.AreEqual<int>(0, handlers.Count());
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForArgumentExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<ArgumentException>();
            Assert.AreEqual<int>(2, handlers.Count());
            Assert.IsTrue(handlers.Any(p => p.GetType().Equals(typeof(Apworks.Tests.Common.ExceptionHandlers.SystemExceptionHandler))));
            Assert.IsTrue(handlers.Any(p => p.GetType().Equals(typeof(Apworks.Tests.Common.ExceptionHandlers.ExceptionExceptionHandler))));
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForSystemExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<SystemException>();
            Assert.AreEqual<int>(1, handlers.Count());
            Assert.IsInstanceOfType(handlers.First(), typeof(Apworks.Tests.Common.ExceptionHandlers.SystemExceptionHandler));
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<Exception>();
            Assert.AreEqual<int>(1, handlers.Count());
            Assert.IsInstanceOfType(handlers.First(), typeof(Apworks.Tests.Common.ExceptionHandlers.ExceptionExceptionHandler));
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForInvalidCastExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<InvalidCastException>();
            Assert.AreEqual<int>(1, handlers.Count());
            Assert.IsInstanceOfType(handlers.First(), typeof(Apworks.Tests.Common.ExceptionHandlers.SystemExceptionHandler));
        }

        [TestMethod]
        public void ExceptionHandling_ExceptionHandlerForInvalidNumericOperationExceptionTest()
        {
            var handlers = ExceptionManager.GetExceptionHandlers<InvalidNumericOperationException>();
            Assert.AreEqual<int>(1, handlers.Count());
            Assert.IsInstanceOfType(handlers.First(), typeof(Apworks.Tests.Common.ExceptionHandlers.InvalidOperationExceptionHandler));
        }

        [TestMethod]
        public void ExceptionHandling_HandleInvalidOperationExceptionTest()
        {
            var handled = ExceptionManager.HandleException<Exception>(new Exception());
            Assert.AreEqual<bool>(true, handled);
        }

        [TestMethod]
        public void ExceptionHandling_InvalidStorageOperationTest()
        {
            bool tmpFileExists = Helper.IsTempFileExists();
            Assert.IsFalse(tmpFileExists);
            using (IStorage storage = application.ObjectContainer.GetService<IStorage>())
            {
                storage.Select<Dummy>();
            }
            tmpFileExists = Helper.IsTempFileExists();
            Assert.IsTrue(tmpFileExists);
            string content = Helper.ReadTempFile();
            Assert.AreEqual<string>("SystemExceptionHandler", content);
        }
    }


    public class InvalidNumericOperationException : InvalidOperationException { }

    [Serializable]
    public class Dummy
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
