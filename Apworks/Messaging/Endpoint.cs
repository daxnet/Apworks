using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public class Endpoint : IEndpoint
    {
        private readonly EndpointAddress address;
        private readonly IInboundTransport inboundTransport;
        private readonly IOutboundTransport outboundTransport;
        private readonly IOutboundTransport errorTransport;
        private readonly IInboundPipeline inboundPipeline;
        private readonly IOutboundPipeline outboundPipeline;
        private readonly IOutboundPipeline errorPipeline;

        public Endpoint(EndpointAddress address,
            IInboundTransport inboundTransport,
            IOutboundTransport outboundTransport,
            IOutboundTransport errorTransport,
            IInboundPipeline inboundPipeline,
            IOutboundPipeline outboundPipeline,
            IOutboundPipeline errorPipeline)
        {
            this.address = address;
            this.inboundTransport = inboundTransport;
            this.outboundTransport = outboundTransport;
            this.errorTransport = errorTransport;
            this.inboundPipeline = inboundPipeline;
            this.outboundPipeline = outboundPipeline;
            this.errorPipeline = errorPipeline;
        }

        #region IEndpoint Members

        public EndpointAddress Address
        {
            get { return this.address; }
        }

        public IInboundTransport InboundTransport
        {
            get { return this.inboundTransport ; }
        }

        public IOutboundTransport OutboundTransport
        {
            get { return this.outboundTransport; }
        }

        public IOutboundTransport ErrorTransport
        {
            get { return this.errorTransport; }
        }

        public IInboundPipeline InboundPipeline
        {
            get { return this.inboundPipeline; }
        }

        public IOutboundPipeline OutboundPipeline
        {
            get { return this.outboundPipeline; }
        }

        public IOutboundPipeline ErrorPipeline
        {
            get { return this.errorPipeline; }
        }

        public void Send(EndpointAddress address, object message)
        {
            Message msg = outboundPipeline.AssembleMessage(message);
            outboundTransport.Send(address, msg);
        }

        public object Receive(EndpointAddress address)
        {
            Message msg = inboundTransport.Receive(address);
            return inboundPipeline.DisassembleMessage(msg);
        }

        #endregion
    }
}
