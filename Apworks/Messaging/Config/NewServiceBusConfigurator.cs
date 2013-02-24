using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Config
{
    public class NewServiceBusConfigurator<TServiceBus> : Configurator, IServiceBusConfigurator
        where TServiceBus : IServiceBus
    {

        public override IConfigurator Configure()
        {
            ObjectContainer.RegisterServiceType<IServiceBus, TServiceBus>();
            return this;
        }
    }
}
