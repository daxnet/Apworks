using System;

namespace Apworks.Tests.Common.AggregateRoots
{
    [Serializable]
    public class SourcedCustomerSnapshot : Snapshots.Snapshot
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Birth { get; set; }
    }
}
