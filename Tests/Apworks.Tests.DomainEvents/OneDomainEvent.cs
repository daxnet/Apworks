using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.DomainEvents
{
    [Serializable]
    public class OneDomainEvent : DomainEvent
    {
    }

    public class OneDomainEventHandler : IDomainEventHandler<OneDomainEvent>
    {
        #region IHandler<OneDomainEvent> Members

        public bool Handle(OneDomainEvent message)
        {
            return true;
        }

        #endregion
    }

    public class OneDomainEventAnotherHandler : IDomainEventHandler<OneDomainEvent>
    {
        #region IHandler<OneDomainEvent> Members

        public bool Handle(OneDomainEvent message)
        {
            return true;
        }

        #endregion
    }


}
