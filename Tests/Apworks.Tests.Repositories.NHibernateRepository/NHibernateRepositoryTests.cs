using System;
using System.Collections.Generic;
using System.Linq;
using Apworks.Application;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Specifications;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apworks.Generators;

namespace Apworks.Tests.Repositories.NHibernateRepository
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class NHibernateRepositoryTests
    {
        private static IApp application;
        private Customer customer;

        private const int pageSize = 20;
        private const int pagingTotalRecords = 97;

        public NHibernateRepositoryTests()
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
            IConfigSource configSource = Helper.ConfigSource_Repositories_NHibernateRepository;
            application = AppRuntime.Create(configSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(Helper.AppInit_Repositories_NHibernateRepository);
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
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Properties["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
            config.Properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            config.Properties["connection.connection_string"] = Helper.ClassicTestDB_SQLExpress_ConnectionString;
            config.Properties["dialect"] = "NHibernate.Dialect.MsSql2008Dialect";
            config.Properties["proxyfactory.factory_class"] = "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu";
            config.AddAssembly(typeof(Customer).Assembly);
            NHibernate.Tool.hbm2ddl.SchemaExport schemaExport = new NHibernate.Tool.hbm2ddl.SchemaExport(config);
            schemaExport.Execute(false, true, false);

            customer = new Customer
            {
                //Id = Guid.NewGuid(),
                Birth = DateTime.Now,
                Email = "daxnet@live.com",
                FirstName = "dax",
                LastName = "net",
                Password = "123456",
                Username = "daxnet"
            };
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [Description("Test the adding of an aggregate root to the repository.")]
        public void NHibernateRepositoryTests_AddAggregateRootToRepositoryTest()
        {
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            customerRepository.Context.Dispose();
            Assert.IsNotNull(customer.ID);
        }

        [TestMethod]
        [Description("Test the retrieving of the aggregate root.")]
        public void NHibernateRepositoryTests_RetrieveAggregateRootTest()
        {
            Guid customerId = Guid.Empty;
            // constructs a new aggregate root and persist it.

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            Assert.IsNotNull(customer.ID);

            // retrieve the aggregate root from persistance.
            customerId = customer.ID;
            var customer2 = customerRepository.GetByKey(customerId);
            customerRepository.Context.Dispose();
            Assert.IsNotNull(customer2);
            Assert.AreEqual(customer2.ID, customerId);
        }

        [TestMethod]
        [Description("Test the retrieving of all aggregate roots.")]
        public void NHibernateRepositoryTests_RetrieveAllAggregateRootTest()
        {
            // construct the aggregate root array.
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID=Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID=Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="cust2", LastName="cust2", Password="123456", Username="cust2"},
                new Customer{ ID=Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="123456", Username="cust3"}
            };

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
            {
                customerRepository.Add(cust);
            }
            customerRepository.Context.Commit();
            customerRepository.Context.Dispose();
            Assert.IsNotNull(customers[0].ID);
            Assert.IsNotNull(customers[1].ID);
            Assert.IsNotNull(customers[2].ID);
            Assert.IsNotNull(customers[3].ID);
        }

        [TestMethod]
        [Description("Test the retrieving of aggregate roots by specification.")]
        public void NHibernateRepositoryTests_RetrieveAggregateRootBySpecificationTest()
        {
            // construct the aggregate root array.
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID=Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID=Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="cust2", LastName="cust2", Password="123456", Username="cust2"},
                new Customer{ ID=Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="123456", Username="cust3"}
            };

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
            {
                customerRepository.Add(cust);
            }
            customerRepository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.Equals("cust2"));
            var c = customerRepository.Find(spec);
            customerRepository.Context.Dispose();
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.ID);
        }

        [TestMethod]
        public void NHibernateRepositoryTests_RetrieveByAndSpecificationTest()
        {
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID=Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID=Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="aaa", LastName="bbb", Password="123456", Username="cust2"},
                new Customer{ ID=Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="654321", Username="cust3"}
            };

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.StartsWith("cust")).And(Specification<Customer>.Eval(p => p.Password == "123456"));
            var c = repository.Find(spec);
            repository.Context.Dispose();
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.ID);
        }

        [TestMethod]
        public void NHibernateRepositoryTests_RetrieveByOrSpecificationTest()
        {
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID=Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID=Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="aaa", LastName="bbb", Password="123456", Username="cust2"},
                new Customer{ ID=Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="654321", Username="cust3"}
            };

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.StartsWith("cust")).Or(Specification<Customer>.Eval(p => p.FirstName == "aaa"));
            var c = repository.FindAll(spec);
            repository.Context.Dispose();
            Assert.IsNotNull(c);
            Assert.AreEqual(3, c.Count());
        }

        [TestMethod]
        [Description("Test the retrieving of aggregate roots by specification.")]
        public void NHibernateRepositoryTests_RetrieveAllAggregateRootBySpecificationTest()
        {
            // construct the aggregate root array.
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID=Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID=Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="cust2", LastName="cust2", Password="123456", Username="cust2"},
                new Customer{ ID=Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="123456", Username="cust3"}
            };

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
            {
                customerRepository.Add(cust);
            }
            customerRepository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.StartsWith("cust"));
            var custs = customerRepository.FindAll(spec);
            customerRepository.Context.Dispose();
            Assert.IsNotNull(custs);
            Assert.AreEqual<int>(3, custs.Count());
        }

        [TestMethod]
        public void NHibernateRepositoryTests_ChangeCustomerNameTest()
        {
            Customer customer = new Customer
            {
                Birth = DateTime.Now.AddYears(-20),
                Email = "daxnet@live.com",
                FirstName = "dax",
                LastName = "net",
                Password = "123456",
                Username = "daxnet"
            };

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            customerRepository.Add(customer);
            customerRepository.Context.Commit();
            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.Username.Equals("daxnet"));

            var customer2 = customerRepository.Find(spec);
            Assert.AreEqual(customer.Username, customer2.Username);

            customer2.FirstName = "qingyang";
            customer2.LastName = "chen";
            customerRepository.Update(customer2);
            customerRepository.Context.Commit();


            var customer3 = customerRepository.Find(spec);
            customerRepository.Context.Dispose();
            Assert.AreEqual("qingyang", customer3.FirstName);
            Assert.AreEqual("chen", customer3.LastName);
        }

        [TestMethod]
        [Description("Test the retrieving of aggregate roots by specification and sorting.")]
        public void NHibernateRepositoryTests_RetrieveAllAggregateRootBySpecificationAndSortingTest()
        {
            // construct the aggregate root array.
            Customer[] customers = new Customer[] 
            { 
                customer,
                new Customer{ ID = Helper.AggregateRootId1, Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ ID = Helper.AggregateRootId2, Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="cust2", LastName="cust2", Password="123456", Username="cust2"},
                new Customer{ ID = Helper.AggregateRootId3, Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="123456", Username="cust3"}
            };

            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
            {
                customerRepository.Add(cust);
            }
            customerRepository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.StartsWith("cust"));
            var custs = customerRepository.FindAll(spec, c => c.Email, Storage.SortOrder.Descending);
            customerRepository.Context.Dispose();
            Assert.IsNotNull(custs);
            Assert.AreEqual<int>(3, custs.Count());
            Assert.AreEqual<string>("cust3@apworks.com", custs.First().Email);
        }

        [TestMethod]
        public void NHibernateRepositoryTests_Paging_NormalTest()
        {
            int pageNumber = 3;
            SequentialIdentityGenerator g = new SequentialIdentityGenerator();
            List<Customer> customers = new List<Customer>();
            for (int i = 1; i <= pagingTotalRecords; i++)
                customers.Add(new Customer
                    {
                        ID = (Guid)g.Next,
                        Birth = DateTime.Now.AddYears(-23),
                        Email = "cust" + i + "@apworks.com",
                        FirstName = "cust" + i,
                        LastName = "cust" + i,
                        Password = i.ToString(),
                        Username = "cust" + i,
                        Sequence = i
                    });

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(c => c.FirstName.StartsWith("cust"));

            var result = repository.FindAll(spec, p => p.Sequence, Storage.SortOrder.Ascending, pageNumber, pageSize);
            Assert.AreEqual<int>(pageSize, result.Count());
            Assert.AreEqual<string>(string.Format("cust{0}", (pageNumber - 1) * pageSize + 1), result.First().FirstName);
            Assert.AreEqual<string>(string.Format("cust{0}", pageSize * pageNumber), result.Last().FirstName);
            repository.Context.Dispose();
        }

        [TestMethod]
        public void NHibernateRepositoryTests_Paging_FirstPageTest()
        {
            int pageNumber = 1;
            SequentialIdentityGenerator g = new SequentialIdentityGenerator();
            List<Customer> customers = new List<Customer>();
            for (int i = 1; i <= pagingTotalRecords; i++)
                customers.Add(new Customer
                {
                    ID = (Guid)g.Next,
                    Birth = DateTime.Now.AddYears(-23),
                    Email = "cust" + i + "@apworks.com",
                    FirstName = "cust" + i,
                    LastName = "cust" + i,
                    Password = i.ToString(),
                    Username = "cust" + i,
                    Sequence = i
                });

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(c => c.FirstName.StartsWith("cust"));

            var result = repository.FindAll(spec, p => p.Sequence, Storage.SortOrder.Ascending, pageNumber, pageSize);
            Assert.AreEqual<int>(pageSize, result.Count());
            Assert.AreEqual<string>(string.Format("cust{0}", (pageNumber - 1) * pageSize + 1), result.First().FirstName);
            Assert.AreEqual<string>(string.Format("cust{0}", pageSize * pageNumber), result.Last().FirstName);
            repository.Context.Dispose();
        }

        [TestMethod]
        public void NHibernateRepositoryTests_Paging_LastPageTest()
        {
            int pageNumber = pagingTotalRecords / pageSize + 1;
            List<Customer> customers = new List<Customer>();
            SequentialIdentityGenerator g = new SequentialIdentityGenerator();
            for (int i = 1; i <= pagingTotalRecords; i++)
                customers.Add(new Customer
                {
                    ID = (Guid)g.Next,
                    Birth = DateTime.Now.AddYears(-23),
                    Email = "cust" + i + "@apworks.com",
                    FirstName = "cust" + i,
                    LastName = "cust" + i,
                    Password = i.ToString(),
                    Username = "cust" + i,
                    Sequence = i
                });

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();


            ISpecification<Customer> spec = Specification<Customer>.Eval(c => c.FirstName.StartsWith("cust"));

            var result = repository.FindAll(spec, p => p.Sequence, Storage.SortOrder.Ascending, pageNumber, pageSize);
            Assert.AreEqual<int>(pagingTotalRecords % pageSize, result.Count());
            Assert.AreEqual<string>(string.Format("cust{0}", (pageNumber - 1) * pageSize + 1), result.First().FirstName);
            Assert.AreEqual<string>(string.Format("cust{0}", (pageSize * (pageNumber - 1)) + (pagingTotalRecords % pageSize)), result.Last().FirstName);
            repository.Context.Dispose();
        }
    }
}
