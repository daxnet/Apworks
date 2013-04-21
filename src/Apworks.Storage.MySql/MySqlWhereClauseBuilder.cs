using Apworks.Storage.Builders;

namespace Apworks.Storage.MySql
{
    /// <summary>
    /// Represents the where clause builder for MySql database.
    /// </summary>
    /// <typeparam name="TDataObject">The type of the data object which would be mapped to
    /// a certain table in the relational database.</typeparam>
    internal sealed class MySqlWhereClauseBuilder<TDataObject> : WhereClauseBuilder<TDataObject>
        where TDataObject : class, new()
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>MySqlWhereClauseBuilder&lt;TDataObject&gt;</c> class.
        /// </summary>
        /// <param name="mappingResolver">The <c>Apworks.Storage.IStorageMappingResolver</c>
        /// instance which will be used for generating the mapped field names.</param>
        public MySqlWhereClauseBuilder(IStorageMappingResolver mappingResolver)
            : base(mappingResolver)
        { }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a <c>System.Char</c> value which represents the leading character to be used by the
        /// database parameter.
        /// </summary>
        protected override char ParameterChar
        {
            get { return '?'; }
        }
        #endregion
    }
}
