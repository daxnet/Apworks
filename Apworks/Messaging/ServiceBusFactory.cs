using Apworks.Messaging.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public static class ServiceBusFactory
    {
        public static IServiceBusConfigurator ConfigureServiceBus()
        {
            return new NewServiceBusConfigurator<ServiceBus>();
        }

        public static IServiceBusConfigurator ConfigureServiceBus<TServiceBus>()
            where TServiceBus : IServiceBus
        {
            return new NewServiceBusConfigurator<TServiceBus>();
        }

        public static void Test()
        {
            var bus = ServiceBusFactory.ConfigureServiceBus().UseXmlForMessageBodySerialization().UseBinaryForMessageSerialization().CreateNew();
        }
    }
}
