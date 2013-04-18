using System;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class Customer : IAggregateRoot
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime Birth { get; set; }
        public virtual int Sequence { get; set; }

        #region Public Methods
        /// <summary>
        /// Returns the hash code for current aggregate root.
        /// </summary>
        /// <returns>The calculated hash code for the current aggregate root.</returns>
        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.ID.GetHashCode());
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Apworks.ISourcedAggregateRoot"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            Customer other = obj as Customer;
            if ((object)other == (object)null)
                return false;
            return other.ID == this.ID;
        }
        #endregion

        #region IEntity Members
        /// <summary>
        /// Gets or sets the identifier of the aggregate root.
        /// </summary>
        public virtual Guid ID { get; set; }
        #endregion

    }
}
