using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public class Subscription
    {
        public Subscription() { }
        public Subscription(Type messageType, EndpointAddress endpoint)
        {
        }

        private EndpointAddress endpoint;

        public EndpointAddress Endpoint
        {
            get { return endpoint; }
            set { endpoint = value; }
        }

        private Type messageType;

        public Type MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            Subscription other = obj as Subscription;
            if ((object)other == null)
                return false;
            return this.endpoint == other.endpoint && 
                this.messageType == other.messageType;
        }

        public override int GetHashCode()
        {
            var a = this.endpoint == null ? base.GetHashCode() : this.endpoint.GetHashCode();
            var b = this.messageType == null ? base.GetHashCode() : this.messageType.GetHashCode();
            return Utils.GetHashCode(a, b);
        }

        public override string ToString()
        {
            if (this.endpoint != null &&
                this.messageType != null)
                return string.Format("Endpoint: {0}, Message Type: {1}",
                    this.endpoint, this.messageType);
            return base.ToString();
        }
    }
}
