using System;
using Apworks.Storage;
using Apworks.Storage.General;

namespace Apworks.Queries.Storage.General
{
    /// <summary>
    /// Represents the SQL Server query object storage.
    /// </summary>
    [Obsolete(@"This class is obsolete, query facilities will be provided in other products, 
or users should use their own mechanism to handle query object storages.")]
    public class SqlQueryObjectStorage : SqlStorage, IQueryObjectStorage
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SqlQueryObjectStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mappingResolver">The mapping resolver.</param>
        public SqlQueryObjectStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        { }
        #endregion
    }
}
