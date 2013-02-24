using Apworks.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Pipelines
{
    public abstract class Pipeline : IPipeline
    {
        private readonly IMessageSerializer messageBodySerializer;

        public Pipeline()
            : this(new MessageXmlSerializer())
        {

        }

        public Pipeline(IMessageSerializer messageBodySerializer)
        {
            this.messageBodySerializer = messageBodySerializer;
        }

        

        #region IPipeline Members

        public IMessageSerializer MessageBodySerializer
        {
            get { return this.messageBodySerializer; }
        }

        #endregion
    }
}
