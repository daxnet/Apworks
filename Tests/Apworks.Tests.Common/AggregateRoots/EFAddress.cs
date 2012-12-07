using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class EFAddress
    {
        #region Private Fields
        private string country;
        private string state;
        private string city;
        private string street;
        private string zip;
        #endregion

        #region Public Static Fields
        /// <summary>
        /// Gets the instance of the <c>Address</c> class which represents an empty address value.
        /// </summary>
        public static readonly EFAddress Emtpy = new EFAddress(null, null, null, null, null);
        #endregion

        #region Ctor
        public EFAddress() { }

        /// <summary>
        /// Initializes a new instance of <c>Address</c> class.
        /// </summary>
        /// <param name="country">The Country part of the address.</param>
        /// <param name="state">The state part of the address.</param>
        /// <param name="city">The City part of the address.</param>
        /// <param name="street">The Street part of the address.</param>
        /// <param name="zip">The Zip part of the address.</param>
        public EFAddress(string country, string state, string city, string street, string zip)
        {
            this.country = country;
            this.state = state;
            this.city = city;
            this.street = street;
            this.zip = zip;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the Country part of the address.
        /// </summary>
        public string Country
        {
            get { return country; }
            set { this.country = value; }
        }
        /// <summary>
        /// Gets or sets the State part of the address.
        /// </summary>
        public string State
        {
            get { return state; }
            set { this.state = value; }
        }
        /// <summary>
        /// Gets or sets the City part of the address.
        /// </summary>
        public string City
        {
            get { return city; }
            set { this.city = value; }
        }
        /// <summary>
        /// Gets or sets the Street part of the address.
        /// </summary>
        public string Street
        {
            get { return street; }
            set { this.street = value; }
        }
        /// <summary>
        /// Gets or sets the Zip part of the address.
        /// </summary>
        public string Zip
        {
            get { return zip; }
            set { this.zip = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if the given object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>True if the two are identical, otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            if (!(obj is EFAddress))
                return false;
            EFAddress other = (EFAddress)obj;
            if (other == null)
                return false;
            return this.country.Equals(other.country) &&
                this.state.Equals(other.state) &&
                this.city.Equals(other.city) &&
                this.street.Equals(other.street) &&
                this.zip.Equals(other.zip);
        }
        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.country.GetHashCode() ^
                this.state.GetHashCode() ^
                this.city.GetHashCode() ^
                this.street.GetHashCode() ^
                this.zip.GetHashCode();
        }
        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}, {2}, {3}, {4}", zip, street, city, state, country);
        }
        #endregion

        #region Public Static Operator Overrides
        /// <summary>
        /// Checks if the one object is equal to another object.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <returns>True if the two are identical, otherwise, false.</returns>
        public static bool operator ==(EFAddress a, EFAddress b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }
            return a.Equals(b);
        }
        /// <summary>
        /// Checks if the one object is not equal to another object.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <returns>True if the two are not identical, otherwise, false.</returns>
        public static bool operator !=(EFAddress a, EFAddress b)
        {
            return !(a == b);
        }
        #endregion
    }
}
