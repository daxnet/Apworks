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
        public int Sequence { get; set; }

        #region IEntity Members

        public Guid ID
        {
            get;
            set;
        }

        #endregion
    }
}
