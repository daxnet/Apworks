using System.Data.Common;
using System.Data.SqlClient;
using Apworks.Storage.Builders;
using Apworks.Storage.General.Builders;

namespace Apworks.Storage.General
{
    /// <summary>
    /// Represents the SQL Server storage.
    /// </summary>
    public class SqlStorage : RdbmsStorage
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SqlStorage</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mappingResolver">The mapping resolver.</param>
        public SqlStorage(string connectionString, IStorageMappingResolver mappingResolver)
            : base(connectionString, mappingResolver)
        {
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates the database connection.
        /// </summary>
        /// <returns>The <see cref="System.Data.Common.DbConnection"/> instance which represents
        /// the open connection to the relational database.</returns>
        protected override DbConnection CreateDatabaseConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }
        /// <summary>
        /// Creates a database parameter object.
        /// </summary>
        /// <returns>The instance of database parameter object.</returns>
        protected override DbParameter CreateParameter()
        {
            return new SqlParameter();
        }
        /// <summary>
        /// Creates a instance of the command object.
        /// </summary>
        /// <param name="sql">The SQL statement used for creating the command object.</param>
        /// <param name="connection">The database connection instance.</param>
        /// <returns>The instance of the command object.</returns>
        protected override DbCommand CreateCommand(string sql, DbConnection connection)
        {
            SqlCommand command = new SqlCommand(sql, connection as SqlConnection);
            return command;
        }
        /// <summary>
        /// Creates a new instance of the where clause builder.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The instance of the where clause builder.</returns>
        protected override WhereClauseBuilder<T> CreateWhereClauseBuilder<T>()
        {
            return new SqlWhereClauseBuilder<T>(MappingResolver);
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
            get { return true; }
        }
        #endregion
    }
}
