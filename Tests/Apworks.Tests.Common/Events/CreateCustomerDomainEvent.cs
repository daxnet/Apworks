using System;
using Apworks.Events;

namespace Apworks.Tests.Common.Events
{
    [Serializable]
    public class CreateCustomerDomainEvent : DomainEvent
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
