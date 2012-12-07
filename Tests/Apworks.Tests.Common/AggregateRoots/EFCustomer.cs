using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class EFCustomer : IAggregateRoot
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public EFAddress Address { get; set; }
        public string Email { get; set; }

        #region IEntity Members

        public Guid ID
        {
            get;
            set;
        }

        #endregion

        #region IEquatable<IEntity> Members

        public bool Equals(IEntity other)
        {
            return other != null && other.GetType() == typeof(EFCustomer) && this.ID == other.ID;
        }

        #endregion
    }
}
