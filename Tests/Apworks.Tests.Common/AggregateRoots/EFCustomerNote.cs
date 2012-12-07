using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class EFCustomerNote : IAggregateRoot
    {

        public EFCustomer Customer { get; set; }
        public List<EFNote> Notes { get; set; }

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
            return other != null && other.GetType() == typeof(EFCustomerNote) && other.ID == this.ID;
        }

        #endregion
    }
}
