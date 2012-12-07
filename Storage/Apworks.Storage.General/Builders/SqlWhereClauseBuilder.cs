
using Apworks.Storage.Builders;

namespace Apworks.Storage.General.Builders
{
    /// <summary>
    /// Represents the WHERE clause builder for SQL Server database.
    /// </summary>
    /// <typeparam name="T">The type of the object that will be mapped to a single SQL Server data table.</typeparam>
    public sealed class SqlWhereClauseBuilder<T> : WhereClauseBuilder<T>
        where T : class, new()
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SqlWhereClauseBuilder&lt;T&gt;</c> class.
        /// </summary>
        /// <param name="mappingResolver">The storage mapping resolver instance.</param>
        public SqlWhereClauseBuilder(IStorageMappingResolver mappingResolver)
            : base(mappingResolver)
        { }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a <c>System.Char</c> value which represents the leading character to be used by the
        /// SQL Server parameter.
        /// </summary>
        protected override char ParameterChar
        {
            get { return '@'; }
        }
        #endregion
    }
}
