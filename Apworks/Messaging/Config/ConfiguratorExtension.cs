using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Config
{
    public static class ConfiguratorExtension
    {
        public static IMessageBodySerializerConfigurator UseXmlForMessageBodySerialization(this IServiceBusConfigurator configurator)
        {
            return new MessageBodySerializerConfigurator<MessageXmlSerializer>(configurator);
        }

        public static IMessageBodySerializerConfigurator UseBinaryForMessageBodySerialization(this IServiceBusConfigurator configurator)
        {
            return new MessageBodySerializerConfigurator<MessageBinarySerializer>(configurator);
        }

        public static IMessageBodySerializerConfigurator UseDataContractForMessageBodySerialization(this IServiceBusConfigurator configurator)
        {
            return new MessageBodySerializerConfigurator<MessageDataContractSerializer>(configurator);
        }

        public static IMessageBodySerializerConfigurator UseJsonForMessageBodySerialization(this IServiceBusConfigurator configurator)
        {
            return new MessageBodySerializerConfigurator<MessageJsonSerializer>(configurator);
        }

        public static IMessageSerializerConfigurator UseXmlForMessageSerialization(this IMessageBodySerializerConfigurator configurator)
        {
            return new MessageSerializerConfigurator<MessageXmlSerializer>(configurator);
        }

        public static IMessageSerializerConfigurator UseBinaryForMessageSerialization(this IMessageBodySerializerConfigurator configurator)
        {
            return new MessageSerializerConfigurator<MessageBinarySerializer>(configurator);
        }

        public static IMessageSerializerConfigurator UseDataContractForMessageSerialization(this IMessageBodySerializerConfigurator configurator)
        {
            return new MessageSerializerConfigurator<MessageDataContractSerializer>(configurator);
        }

        public static IMessageSerializerConfigurator UseJsonForMessageSerialization(this IMessageBodySerializerConfigurator configurator)
        {
            return new MessageSerializerConfigurator<MessageJsonSerializer>(configurator);
        }

        public static IServiceBus CreateNew(this IMessageSerializerConfigurator configurator)
        {
            return (configurator as Configurator).ObjectContainer.GetService<IServiceBus>();
        }
    }
}
