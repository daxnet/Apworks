
namespace Apworks.Tests.Common.MessageHandlers
{
    public class EmailChangedEventHandler : Apworks.Events.EventHandler<Events.ChangeEmailDomainEvent>
    {
        public override bool Handle(Events.ChangeEmailDomainEvent message)
        {
            return true;
        }
    }
}
