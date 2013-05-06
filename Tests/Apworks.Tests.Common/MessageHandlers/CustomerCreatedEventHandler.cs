using Apworks.Events;

namespace Apworks.Tests.Common.MessageHandlers
{
    public class CustomerCreatedEventHandler : IEventHandler<Events.CreateCustomerDomainEvent>
    {
        #region IHandler<CreateCustomerDomainEvent> Members

        public void Handle(Events.CreateCustomerDomainEvent message)
        {

        }

        #endregion

        //#region IHandler Members

        //public bool Handle(object message)
        //{
        //    return true;
        //}

        //#endregion
    }
}
