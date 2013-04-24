using System.Data.Entity;
using Apworks.Tests.Common.AggregateRoots;
using System.ComponentModel.DataAnnotations.Schema;

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFCustomer>().HasKey(p => p.ID);
            modelBuilder.Entity<EFCustomer>().Property(p => p.ID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);
        }
    }
}
