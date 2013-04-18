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

    }
}
