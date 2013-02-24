using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public interface IOutboundPipeline : IPipeline
    {
        Message AssembleMessage(object message);
    }
}
