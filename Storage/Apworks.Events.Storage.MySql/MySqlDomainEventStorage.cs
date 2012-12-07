using Apworks.Storage;
using Apworks.Storage.MySql;

namespace Apworks.Events.Storage.MySql
{
    /// <summary>
    /// Represents the domain event storage that uses MySQL as the storage device.
    /// </summary>
    public class MySqlDomainEventStorage : RdbmsDomainEventStorage<MySqlStorage>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MySqlDomainEventStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mappingResolver">The mapping resolver.</param>
        public MySqlDomainEventStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        {

        }
        #endregion
    }
}
