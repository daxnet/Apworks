using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Config
{
    internal class MessageSerializerConfigurator<TMessageSerializer> : Configurator, IMessageSerializerConfigurator
        where TMessageSerializer : IMessageSerializer
    {
        private readonly IMessageBodySerializerConfigurator configurator;

        public MessageSerializerConfigurator(IMessageBodySerializerConfigurator configurator)
        {
            this.configurator = configurator;
        }

        public override IConfigurator Configure()
        {
            ObjectContainer.RegisterServiceType<IMessageSerializer, TMessageSerializer>("messageSerializer");
            return this;
        }
    }
}
