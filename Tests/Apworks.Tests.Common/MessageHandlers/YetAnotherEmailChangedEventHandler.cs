
namespace Apworks.Tests.Common.MessageHandlers
{
    public class YetAnotherEmailChangedEventHandler : Apworks.Events.IEventHandler<Events.ChangeEmailDomainEvent>
    {
        public void Handle(Events.ChangeEmailDomainEvent message)
        {
            throw new System.Exception(""); // simulate process failed.
        }
    }
}
