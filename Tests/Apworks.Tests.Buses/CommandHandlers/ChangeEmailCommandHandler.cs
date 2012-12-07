using Apworks.Commands;
using Apworks.Repositories;
using Apworks.Tests.Buses.Commands;
using Apworks.Tests.Common.AggregateRoots;

namespace Apworks.Tests.Buses.CommandHandlers
{
    public class ChangeEmailCommandHandler : CommandHandler<ChangeEmailCommand>
    {
        public ChangeEmailCommandHandler()
        {
        }

        public override bool Handle(ChangeEmailCommand command)
        {
            using (IDomainRepository repository = this.DomainRepository)
            {
                var cust = repository.Get<SourcedCustomer>(command.CustomerId);
                for (int i = 0; i < 1005; i++)
                {
                    cust.ChangeEmail(command.NewEmail);
                }
                repository.Save<SourcedCustomer>(cust);
                repository.Commit();
                return true;
            }
        }
    }
}
