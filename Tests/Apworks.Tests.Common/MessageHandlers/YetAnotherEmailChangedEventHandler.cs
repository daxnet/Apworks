
namespace Apworks.Tests.Common.MessageHandlers
{
    public class YetAnotherEmailChangedEventHandler : Apworks.Events.EventHandler<Events.ChangeEmailDomainEvent>
    {
        public override bool Handle(Events.ChangeEmailDomainEvent message)
        {
            return false;
        }
    }
}
