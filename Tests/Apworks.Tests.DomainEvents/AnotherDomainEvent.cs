using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.DomainEvents
{
    [Serializable]
    public class AnotherDomainEvent : DomainEvent
    {
    }

    public class AnotherDomainEventHandler : IDomainEventHandler<AnotherDomainEvent>
    {
        #region IHandler<AnotherDomainEvent> Members

        public bool Handle(AnotherDomainEvent message)
        {
            return true;
        }

        #endregion
    }

}
