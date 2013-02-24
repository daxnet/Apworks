using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IOutboundTransport : ITransport
    {
        void Send(EndpointAddress endpoint, Message message);
    }
}
