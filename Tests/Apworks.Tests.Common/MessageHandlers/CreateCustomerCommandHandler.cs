using Apworks.Commands;

namespace Apworks.Tests.Common.MessageHandlers
{
    public class CreateCustomerCommandHandler : CommandHandler<Commands.CreateCustomerCommand>
    {
        public override bool Handle(Commands.CreateCustomerCommand command)
        {
            return true;
        }
    }
}
