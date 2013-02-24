using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    internal class ServiceBus : IServiceBus
    {
        private readonly IEndpoint endpoint;
        private readonly ISubscriptionStorage subscriptionStorage;

        public ServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
        {
            this.endpoint = endpoint;
            this.subscriptionStorage = subscriptionStorage;
        }

        #region IServiceBus Members

        public IEndpoint Endpoint
        {
            get { return this.endpoint; }
        }

        public void Publish(object message)
        {
            var subscriptions = subscriptionStorage.GetSubscriptions(message.GetType());
            foreach (var subscription in subscriptions)
                this.endpoint.Send(subscription.Endpoint, message);
        }

        #endregion
    }
}
