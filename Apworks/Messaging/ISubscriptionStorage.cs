using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface ISubscriptionStorage
    {
        void Add(Subscription subscription);
        void Remove(Subscription subscription);
        IEnumerable<Subscription> Subscriptions { get; }
        IEnumerable<Subscription> GetSubscriptions(Type messageType);
        void Clear();
        int Count { get; }
    }
}
