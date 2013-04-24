using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apworks.Application;
using Apworks.Config;
using Apworks.Tests.Common;
using Apworks.Tests.Common.AggregateRoots;
using Apworks.Repositories;
using MongoDB.Driver;
using Apworks.Specifications;
using MongoDB.Driver.Builders;
using Apworks.Storage;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apworks.Tests.Repositories.MongoDB
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MongoDBRepositoryTests
    {
        private static IApp application;
        public MongoDBRepositoryTests()
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
            IConfigSource configSource = Helper.ConfigSource_MongoDBRepository;
            application = AppRuntime.Create(configSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(Helper.AppInit_MongoDBRepository);
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
            Helper.ClearMongoDB();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MongoDBRepositoryTests_InsertDocument()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                    {
                        FirstName = "sunny" + i.ToString(),
                        LastName = "chen" + i.ToString(),
                        Birth = DateTime.Now.AddDays(-i),
                        Email = "sunnychen" + i.ToString() + "@163.com",
                        Password = i.ToString(),
                        Sequence = i,
                        ID = Guid.NewGuid(),
                        Username = "sunnychen" + i.ToString()
                    });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new {context = context});
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            MongoClient client = new MongoClient(Helper.MongoDB_ConnectionString);
            //MongoServer server = MongoServer.Create(Helper.MongoDB_ConnectionString);
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase(Helper.MongoDB_Database);
            MongoCollection collection = database.GetCollection("Customer");
            var count = collection.Count();
            Assert.AreEqual(100, count);
        }

        [TestMethod]
        public void MongoDBRepositoryTests_ModifyDocument()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            Customer oneCustomer = customerRepository.Find(Specification<Customer>.Eval(c => c.Sequence == 50));
            oneCustomer.FirstName = "daxnet";
            customerRepository.Update(oneCustomer);
            context.Commit();

            MongoClient client = new MongoClient(Helper.MongoDB_ConnectionString);
            //MongoServer server = MongoServer.Create(Helper.MongoDB_ConnectionString);
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase(Helper.MongoDB_Database);
            MongoCollection collection = database.GetCollection("Customer");
            var query = Query.EQ("Sequence", 50);
            var modifiedCustomer = collection.FindOneAs<Customer>(query);
            Assert.AreEqual<string>("daxnet", modifiedCustomer.FirstName);
        }

        [TestMethod]
        public void MongoDBRepositoryTests_DeleteDocument()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            Customer oneCustomer = customerRepository.Find(Specification<Customer>.Eval(c => c.Sequence == 50));
            customerRepository.Remove(oneCustomer);
            context.Commit();

            MongoClient client = new MongoClient(Helper.MongoDB_ConnectionString);
            //MongoServer server = MongoServer.Create(Helper.MongoDB_ConnectionString);
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase(Helper.MongoDB_Database);
            MongoCollection collection = database.GetCollection("Customer");
            var query = Query.EQ("Sequence", 50);
            var deletedCustomer = collection.FindOneAs<Customer>(query);
            Assert.IsNull(deletedCustomer);
            Assert.AreEqual(99, collection.Count());
        }

        [TestMethod]
        public void MongoDBRepositoryTests_FindAll()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            var foundCustomers = customerRepository.FindAll();
            Assert.AreEqual(100, foundCustomers.Count());
        }

        [TestMethod]
        public void MongoDBRepositoryTests_FindAllBySpecification()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            var foundCustomers = customerRepository.FindAll(Specification<Customer>.Eval(p => p.Sequence % 2 == 0));
            Assert.AreEqual(50, foundCustomers.Count());
        }

        [TestMethod]
        public void MongoDBRepositoryTests_RetrieveByAndSpecification()
        {
            Customer[] customers = new Customer[] 
            { 
                new Customer{ Birth=DateTime.Now.AddYears(-23), Email="cust1@apworks.com", FirstName="cust1", LastName="cust1", Password="123456", Username="cust1"},
                new Customer{ Birth=DateTime.Now.AddYears(-23), Email="cust2@apworks.com", FirstName="aaa", LastName="bbb", Password="123456", Username="cust2"},
                new Customer{ Birth=DateTime.Now.AddYears(-23), Email="cust3@apworks.com", FirstName="cust3", LastName="cust3", Password="654321", Username="cust3"}
            };

            IRepository<Customer> repository = ServiceLocator.Instance.GetService<IRepository<Customer>>();
            foreach (var cust in customers)
                repository.Add(cust);
            repository.Context.Commit();

            ISpecification<Customer> spec = Specification<Customer>.Eval(p => p.FirstName.StartsWith("cust")).And(Specification<Customer>.Eval(p => p.Password == "123456"));
            var c = repository.FindAll(spec);
            repository.Context.Dispose();
            Assert.IsNotNull(c);
            Assert.AreEqual(1, c.Count());
        }

        [TestMethod]
        public void MongoDBRepositoryTests_RetrieveByOrSpecificationTest()
        {
            Customer[] customers = new Customer[] 
            { 
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
        public void MongoDBRepositoryTests_FindAllByDescendingSorting()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            var foundCustomers = customerRepository.FindAll(sp => sp.Sequence, SortOrder.Descending);
            Assert.AreEqual(100, foundCustomers.Count());
            var firstCustomer = foundCustomers.First();
            Assert.AreEqual("sunny99", firstCustomer.FirstName);
        }

        [TestMethod]
        public void MongoDBRepositoryTests_FindAllByAscendingSorting()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            var foundCustomers = customerRepository.FindAll(sp => sp.Sequence, SortOrder.Ascending);
            Assert.AreEqual(100, foundCustomers.Count());
            var firstCustomer = foundCustomers.First();
            Assert.AreEqual("sunny0", firstCustomer.FirstName);
        }

        [TestMethod]
        public void MongoDBRepositoryTests_FindAllByPaging()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < 100; i++)
                customers.Add(new Customer
                {
                    FirstName = "sunny" + i.ToString(),
                    LastName = "chen" + i.ToString(),
                    Birth = DateTime.Now.AddDays(-i),
                    Email = "sunnychen" + i.ToString() + "@163.com",
                    Password = i.ToString(),
                    Sequence = i,
                    ID = Guid.NewGuid(),
                    Username = "sunnychen" + i.ToString()
                });
            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
            foreach (var customer in customers)
                customerRepository.Add(customer);
            context.Commit();

            var foundCustomers = customerRepository.FindAll(sp => sp.Sequence, SortOrder.Descending, 5, 15);
            Assert.AreEqual(15, foundCustomers.Count());
            var firstCust = foundCustomers.First();
            var lastCust = foundCustomers.Last();
            Assert.AreEqual(39, firstCust.Sequence);
            Assert.AreEqual(25, lastCust.Sequence);
        }

        #region Test - COMMENTED
        //[TestMethod]
        //public void MongoDBRepositoryTests_Dummy()
        //{
        //    List<Customer> customers = new List<Customer>();
        //    for (int i = 0; i < 100; i++)
        //        customers.Add(new Customer
        //        {
        //            FirstName = "sunny" + i.ToString(),
        //            LastName = "chen" + i.ToString(),
        //            Birth = DateTime.Now.AddDays(-i),
        //            Email = "sunnychen" + i.ToString() + "@163.com",
        //            Password = i.ToString(),
        //            Sequence = i,
        //            ID = Guid.NewGuid(),
        //            Username = "sunnychen" + i.ToString()
        //        });
        //    IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
        //    IRepository<Customer> customerRepository = ServiceLocator.Instance.GetService<IRepository<Customer>>(new { context = context });
        //    foreach (var customer in customers)
        //        customerRepository.Add(customer);
        //    context.Commit();

        //    MongoServer server = MongoServer.Create(Helper.MongoDB_ConnectionString);
        //    MongoDatabase database = server.GetDatabase(Helper.MongoDB_Database);
        //    MongoCollection collection = database.GetCollection("Customer");
        //    Expression<Func<Customer, dynamic>> pred = p => p.Sequence;

        //    var param = pred.Parameters[0];

        //    string propertyName = null;
        //    Type propertyType = typeof(object);
        //    Expression bodyExpression = null;
        //    if (pred.Body is UnaryExpression)
        //    {
        //        UnaryExpression unaryExpression = pred.Body as UnaryExpression;
        //        bodyExpression = unaryExpression.Operand;
        //    }
        //    else if (pred.Body is MemberExpression)
        //    {
        //        bodyExpression = pred.Body;
        //    }
        //    MemberExpression memberExpression = (MemberExpression)bodyExpression;
        //    propertyName = memberExpression.Member.Name;
        //    if (memberExpression.Member.MemberType == MemberTypes.Property)
        //    {
        //        PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
        //        propertyType = propertyInfo.PropertyType;
        //    }

        //    var query = collection.AsQueryable<Customer>().Where(p => true);

        //    var methods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
        //    var orderByDescendingMethod = methods.Where(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2).First();
        //    IOrderedQueryable<Customer> orderedQuery = (IOrderedQueryable<Customer>)orderByDescendingMethod
        //        .MakeGenericMethod(typeof(Customer), propertyType)
        //        .Invoke(null, new object[] { query, GetSortExpression(param, propertyType, propertyName) });
        //    var custs = orderedQuery.ToList();
        //}

        //private object GetSortExpression(ParameterExpression param, Type propertyType, string propertyName)
        //{
        //    object expr = null;

        //    Type funcType = typeof(Func<,>);
        //    funcType = funcType.MakeGenericType(typeof(Customer), propertyType);

        //    expr = Expression.Lambda(funcType, Expression.Convert(
        //            Expression.Property(param, propertyName),
        //            typeof(int)), param);
        //    return expr;
        //}
        #endregion

    }
}
