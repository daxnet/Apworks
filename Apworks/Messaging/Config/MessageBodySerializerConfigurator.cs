using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Config
{
    internal class MessageBodySerializerConfigurator<TMessageSerializer> : Configurator, IMessageBodySerializerConfigurator
        where TMessageSerializer : IMessageSerializer
    {
        private readonly IServiceBusConfigurator configurator;

        public MessageBodySerializerConfigurator(IServiceBusConfigurator configurator)
        {
            this.configurator = configurator;
        }

        public override IConfigurator Configure()
        {
            ObjectContainer.RegisterServiceType<IMessageSerializer, TMessageSerializer>("messageBodySerializer");
            return this;
        }
    }
}
