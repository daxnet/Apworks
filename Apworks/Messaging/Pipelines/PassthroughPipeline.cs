using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Pipelines
{
    public class PassthroughPipeline : Pipeline, IInboundPipeline, IOutboundPipeline
    {
        public PassthroughPipeline() : base() { }
        public PassthroughPipeline(IMessageSerializer messageBodySerializer)
            : base(messageBodySerializer)
        { }

        #region IInboundPipeline Members

        public object DisassembleMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            return MessageBodySerializer.Deserialize(Type.GetType(message.AssemblyQualifiedName), message.Body);
        }

        #endregion

        #region IOutboundPipeline Members

        public Message AssembleMessage(object message)
        {
            return Message.FromObject(message, MessageBodySerializer);
        }

        #endregion
    }
}
