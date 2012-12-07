using System;
using System.Data.SqlTypes;
using Apworks.Snapshots;
using Apworks.Tests.Common.Events;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class SourcedCustomer : SourcedAggregateRoot
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime Birth { get; set; }

        /// <summary>
        /// Initializes a new instance of <c>SourcedCustomer</c> class. With the Username="daxnet", FirstName="Sunny", LastName="Chen".
        /// </summary>
        public SourcedCustomer()
        {
            this.RaiseEvent<CreateCustomerDomainEvent>(new CreateCustomerDomainEvent { Username = "daxnet", FirstName = "Sunny", LastName = "Chen" });
            //this.Username = "daxnet";
            //this.FirstName = "Sunny";
            //this.LastName = "Chen";
            //this.Birth = SqlDateTime.MinValue.Value;
        }


        [Handles(typeof(CreateCustomerDomainEvent))]
        private void CreateCustomerDomainEventHandler(CreateCustomerDomainEvent @event)
        {
            this.Username = @event.Username;
            this.FirstName = @event.FirstName;
            this.LastName = @event.LastName;
            this.Birth = SqlDateTime.MinValue.Value;
        }

        [Handles(typeof(ChangeCustomerNameDomainEvent))]
        private void ChangeCustomerNameDomainEventHandler(ChangeCustomerNameDomainEvent @event)
        {
            this.FirstName = @event.FirstName;
            this.LastName = @event.LastName;
        }

        [Handles(typeof(ChangeEmailDomainEvent))]
        private void ChangeEmailEventHandler(ChangeEmailDomainEvent @event)
        {
            this.Email = @event.Email;
        }

        public virtual void ChangeName(string firstName, string lastName)
        {
            this.RaiseEvent<ChangeCustomerNameDomainEvent>(new ChangeCustomerNameDomainEvent { FirstName = firstName, LastName = lastName });
        }

        public virtual void ChangeEmail(string email)
        {
            this.RaiseEvent<ChangeEmailDomainEvent>(new ChangeEmailDomainEvent { Email = email });
        }


        protected override void DoBuildFromSnapshot(ISnapshot snapshot)
        {
            SourcedCustomerSnapshot s = (SourcedCustomerSnapshot)snapshot;
            this.Birth = s.Birth;
            this.Email = s.Email;
            this.FirstName = s.FirstName;
            this.LastName = s.LastName;
            this.Password = s.Password;
            this.Username = s.Username;
        }

        protected override ISnapshot DoCreateSnapshot()
        {
            return new SourcedCustomerSnapshot
            {
                Birth = this.Birth,
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Password = this.Password,
                Username = this.Username
            };
        }
    }
}
