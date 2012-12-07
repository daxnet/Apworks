using System;
using Apworks.Events;

namespace Apworks.Tests.Common.Events
{
    [Serializable]
    public class ChangeEmailDomainEvent : DomainEvent
    {
        public string Email { get; set; }
    }
}
