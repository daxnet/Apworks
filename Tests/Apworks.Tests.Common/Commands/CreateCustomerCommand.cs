using Apworks.Commands;

namespace Apworks.Tests.Common.Commands
{
    public class CreateCustomerCommand : Command
    {
        private string name;    

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        
    }
}
