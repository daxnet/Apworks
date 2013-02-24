using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging
{
    public class EndpointAddress
    {
        private Uri address;

        public EndpointAddress() { }

        public EndpointAddress(string uriString)
        {
            this.address = new Uri(uriString);
        }

        public EndpointAddress(Uri address)
        {
            this.address = address;
        }

        public Uri Address
        {
            get { return address; }
            set { address = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            EndpointAddress other = obj as EndpointAddress;
            if ((object)other == null)
                return false;
            return this.address.Equals(other.address);
        }

        public override int GetHashCode()
        {
            if (this.address != null)
                return Utils.GetHashCode(this.address.GetHashCode());
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (this.address != null)
                return this.address.ToString();
            return base.ToString();
        }

        public static implicit operator string(EndpointAddress endpoint)
        {
            return endpoint.Address.ToString();
        }

        public static implicit operator EndpointAddress(string uriString)
        {
            return new EndpointAddress(uriString);
        }
    }
}
