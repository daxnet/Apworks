using System;
using Apworks.Events;

namespace Apworks.Tests.Common.Events
{
    [Serializable]
    public class ChangeCustomerNameDomainEvent : DomainEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
