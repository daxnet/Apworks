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

        public void Handle(OneDomainEvent message)
        {

        }

        #endregion
    }

    public class OneDomainEventAnotherHandler : IDomainEventHandler<OneDomainEvent>
    {
        #region IHandler<OneDomainEvent> Members

        public void Handle(OneDomainEvent message)
        {

        }

        #endregion
    }


}
