using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IInboundTransport : ITransport
    {
        Message Receive(EndpointAddress endpoint);
    }
}
