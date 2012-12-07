using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Tests.Common.AggregateRoots
{
    public class EFNote : IEntity
    {

        public string NoteText { get; set; }

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
            return other != null && other.GetType() == typeof(EFNote) && this.ID == other.ID;
        }

        #endregion
    }
}
