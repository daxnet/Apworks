using System.Data.Entity;
using Apworks.Tests.Common.AggregateRoots;

namespace Apworks.Tests.Common.EFContexts
{
    public class EFTestContext : DbContext
    {
        public EFTestContext()
            : base("EFTestContext")
        { }

        public DbSet<EFCustomer> Customers
        {
            get { return Set<EFCustomer>(); }
        }

        public DbSet<EFCustomerNote> CustomerNotes
        {
            get { return Set<EFCustomerNote>(); }
        }
    }
}
