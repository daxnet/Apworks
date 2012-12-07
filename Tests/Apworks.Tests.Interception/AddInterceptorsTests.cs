using System;
using System.Reflection;
using Apworks.Config;
using Apworks.Storage;
using Apworks.Tests.Common;
using Apworks.Tests.Common.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apworks.Tests.Interception
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AddInterceptorsTests
    {
        private readonly Type UnitOfWorkContract = typeof(IUnitOfWork);
        private readonly MethodInfo CommitMethod;
        private readonly MethodInfo RollbackMethod;

        private readonly Type StorageContract = typeof(IStorage);
        private readonly MethodInfo InsertMethod;

        public AddInterceptorsTests()
        {
            CommitMethod = UnitOfWorkContract.GetMethod("Commit", BindingFlags.Public | BindingFlags.Instance);
            RollbackMethod = UnitOfWorkContract.GetMethod("Rollback", BindingFlags.Public | BindingFlags.Instance);
            InsertMethod = StorageContract.GetMethod("Insert", BindingFlags.Public | BindingFlags.Instance);
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
        public void AddInterceptorsTests_AddSingleInterceptorRefToSingleMethodInSingleContractTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            Assert.IsNotNull(configSource.Config.Interception);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.Count);
            Assert.AreEqual<string>(UnitOfWorkContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Type);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.Count);
            Assert.AreEqual<string>(CommitMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddMultipleInterceptorRefsToSingleMethodInSingleContractTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName);
            Assert.IsNotNull(configSource.Config.Interception);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.Count);
            Assert.AreEqual<string>(UnitOfWorkContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Type);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.Count);
            Assert.AreEqual<string>(CommitMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(1).Name);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddSingleInterceptorRefToMultipleMethodInSingleContractTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(UnitOfWorkContract, RollbackMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            Assert.IsNotNull(configSource.Config.Interception);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.Count);
            Assert.AreEqual<string>(UnitOfWorkContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Type);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.Count);
            Assert.AreEqual<string>(CommitMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);
            Assert.AreEqual<string>(RollbackMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).Signature);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).InterceptorRefs.GetItemAt(0).Name);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddMultipleInterceptorRefToMultipleMethodInSingleContractTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(UnitOfWorkContract, RollbackMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(UnitOfWorkContract, RollbackMethod, typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName);
            Assert.IsNotNull(configSource.Config.Interception);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.Count);
            Assert.AreEqual<string>(UnitOfWorkContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Type);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.Count);
            Assert.AreEqual<string>(CommitMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(1).Name);
            Assert.AreEqual<string>(RollbackMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).Signature);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).InterceptorRefs.GetItemAt(0).Name);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(1).InterceptorRefs.GetItemAt(1).Name);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddSingleInterceptorRefToSingleMethodInMultipleContractTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptorRef(UnitOfWorkContract, CommitMethod, typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName);
            configSource.AddInterceptorRef(StorageContract, InsertMethod, typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName);
            Assert.IsNotNull(configSource.Config.Interception);
            Assert.AreEqual<int>(2, configSource.Config.Interception.Contracts.Count);
            Assert.AreEqual<string>(UnitOfWorkContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Type);
            Assert.AreEqual<string>(StorageContract.AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(1).Type);

            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.Count);
            Assert.AreEqual<string>(CommitMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.ExceptionHandlingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(0).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);

            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(1).Methods.Count);
            Assert.AreEqual<string>(InsertMethod.GetSignature(), configSource.Config.Interception.Contracts.GetItemAt(1).Methods.GetItemAt(0).Signature);
            Assert.AreEqual<int>(1, configSource.Config.Interception.Contracts.GetItemAt(1).Methods.GetItemAt(0).InterceptorRefs.Count);
            Assert.AreEqual<string>(typeof(Apworks.Tests.Common.Interceptors.LoggingInterceptor).AssemblyQualifiedName, configSource.Config.Interception.Contracts.GetItemAt(1).Methods.GetItemAt(0).InterceptorRefs.GetItemAt(0).Name);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddSingleInterceptorTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("1", typeof(ExceptionHandlingInterceptor));
            Assert.AreEqual<int>(1, configSource.Config.Interception.Interceptors.Count);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddMultipleInterceptorsTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("1", typeof(ExceptionHandlingInterceptor));
            configSource.AddInterceptor("2", typeof(LoggingInterceptor));
            Assert.AreEqual<int>(2, configSource.Config.Interception.Interceptors.Count);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddMultipleInterceptorsWithSameNameTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("1", typeof(ExceptionHandlingInterceptor));
            configSource.AddInterceptor("1", typeof(LoggingInterceptor));
            Assert.AreEqual<int>(1, configSource.Config.Interception.Interceptors.Count);
        }

        [TestMethod]
        public void AddInterceptorsTests_AddMultipleInterceptorsWithSameTypeTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("1", typeof(ExceptionHandlingInterceptor));
            configSource.AddInterceptor("2", typeof(ExceptionHandlingInterceptor));
            Assert.AreEqual<int>(1, configSource.Config.Interception.Interceptors.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void AddInterceptorsTests_AddInvalidInterceptorTypeTest()
        {
            RegularConfigSource configSource = (RegularConfigSource)Helper.ConfigSource_GeneralInterception;
            configSource.AddInterceptor("1", this.GetType());
        }
    }
}
