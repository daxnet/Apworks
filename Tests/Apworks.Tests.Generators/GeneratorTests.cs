using Apworks.Application;
using Apworks.Config;
using Apworks.Generators;
using Apworks.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Apworks.Tests.Generators
{
    /// <summary>
    /// Summary description for IdentityGeneratorTests
    /// </summary>
    [TestClass]
    public class GeneratorTests
    {
        private static IApp application;
        public GeneratorTests()
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
            IConfigSource configSource = Helper.ConfigSource_Generators;
            application = AppRuntime.Create(configSource);
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
        [Description("Test the generating of the identity.")]
        public void Generators_IdentityGenerationTest()
        {
            Guid t = (Guid)IdentityGenerator.Instance.Generate();
            Assert.AreNotEqual<Guid>(Guid.Empty, t);
        }

        [TestMethod]
        [Description("Test the generating of the sequence.")]
        public void Generators_SequenceGenerationTest()
        {
            Guid seq = (Guid)SequenceGenerator.Instance.Next;
            Assert.AreNotEqual<Guid>(Guid.Empty, seq);
        }

        //[TestMethod]
        //public void SequenceTest()
        //{
        //    long prev = 0;
        //    bool hasLower = false;
        //    for (int i = 0; i < 5000000; i++)
        //    {
        //        long seq = (long)SequenceGenerator.Instance.Next;
        //        if (seq <= prev)
        //        {
        //            hasLower = true;
        //            break;
        //        }
        //        prev = seq;
        //    }
        //    Assert.AreEqual<bool>(false, hasLower);
        //}
    }
}
