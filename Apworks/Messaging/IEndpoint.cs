using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IEndpoint
    {
        EndpointAddress Address { get; }
        IInboundTransport InboundTransport { get; }
        IOutboundTransport OutboundTransport { get; }
        IOutboundTransport ErrorTransport { get; }
        IInboundPipeline InboundPipeline { get; }
        IOutboundPipeline OutboundPipeline { get; }
        IOutboundPipeline ErrorPipeline { get; }
        void Send(EndpointAddress address, object message);
        object Receive(EndpointAddress address);
    }
}
