using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IServiceBus
    {
        IEndpoint Endpoint { get; }
        void Publish(object message);
    }
}
