using System;
using Apworks.Storage;
using Apworks.Storage.MySql;

namespace Apworks.Queries.Storage.MySql
{
    /// <summary>
    /// Represents the MySQL query object storage.
    /// </summary>
    [Obsolete(@"This class is obsolete, query facilities will be provided in other products, 
or users should use their own mechanism to handle query object storages.")]
    public class MySqlQueryObjectStorage : MySqlStorage, IQueryObjectStorage
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MySqlQueryObjectStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string which is used by the database engine to 
        /// connect to the specified database server.</param>
        /// <param name="mappingResolver">The storage mapping resolver.</param>
        public MySqlQueryObjectStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        { }
        #endregion
    }
}
