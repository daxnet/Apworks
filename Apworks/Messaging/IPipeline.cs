using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IPipeline
    {
        IMessageSerializer MessageBodySerializer { get; }
    }
}
