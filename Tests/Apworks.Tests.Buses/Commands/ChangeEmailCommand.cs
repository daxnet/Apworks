using Apworks.Commands;
using System;

namespace Apworks.Tests.Buses.Commands
{
    public class ChangeEmailCommand : Command
    {
        public string NewEmail { get; set; }
        public Guid CustomerId { get; set; }
        public ChangeEmailCommand(Guid customerId, string newEmail)
        {
            CustomerId = customerId;
            NewEmail = newEmail;
        }
    }
}
