using System.Data.Common;
using Apworks.Storage.Builders;
using MySql.Data.MySqlClient;

namespace Apworks.Storage.MySql
{
    /// <summary>
    /// Represents the MySql storage.
    /// </summary>
    public class MySqlStorage : RdbmsStorage
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MySqlStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mappingResolver">The mapping resolver.</param>
        public MySqlStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        { }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates a new instance of the where clause builder.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The instance of the where clause builder.</returns>
        protected override WhereClauseBuilder<T> CreateWhereClauseBuilder<T>()
        {
            return new MySqlWhereClauseBuilder<T>(this.MappingResolver);
        }
        /// <summary>
        /// Creates the database connection.
        /// </summary>
        /// <returns>The <see cref="System.Data.Common.DbConnection"/> instance which represents
        /// the open connection to the relational database.</returns>
        protected override DbConnection CreateDatabaseConnection()
        {
            return new MySqlConnection(this.ConnectionString);
        }
        /// <summary>
        /// Creates a database parameter object.
        /// </summary>
        /// <returns>The instance of database parameter object.</returns>
        protected override DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }
        /// <summary>
        /// Creates a instance of the command object.
        /// </summary>
        /// <param name="sql">The SQL statement used for creating the command object.</param>
        /// <param name="connection">The database connection instance.</param>
        /// <returns>The instance of the command object.</returns>
        protected override DbCommand CreateCommand(string sql, DbConnection connection)
        {
            return new MySqlCommand(sql, connection as MySqlConnection);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public override bool DTCompatible
        {
            get { return false; }
        }
        #endregion
    }
}
