using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface ITransport
    {
        /// <summary>
        /// Gets the endpoint of the transport.
        /// </summary>
        bool Transactional { get; }
        IMessageSerializer MessageSerializer { get; }
    }
}
