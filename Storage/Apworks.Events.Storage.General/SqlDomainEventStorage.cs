using Apworks.Storage;
using Apworks.Storage.General;

namespace Apworks.Events.Storage.General
{
    /// <summary>
    /// Represents the domain event storage that uses SQL Server as the storage device.
    /// </summary>
    public class SqlDomainEventStorage : RdbmsDomainEventStorage<SqlStorage>// SqlStorage, IDomainEventStorage
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SqlDomainEventStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mappingResolver">The mapping resolver.</param>
        public SqlDomainEventStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        {
        }
        #endregion
    }
}
