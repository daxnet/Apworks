using NHibernate.Cfg;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the <see cref="Configuration"/> being used
    /// by the <see cref="Apworks.Repositories.NHibernate.NHibernateContext"/>
    /// which uses either app/web.config or external XML source to initialize the NHibernate framework.
    /// </summary>
    public class NHibernateApplicationConfiguration : Configuration
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>NHibernateApplicationConfiguration</c> class.
        /// </summary>
        public NHibernateApplicationConfiguration()
            : base()
        {
            this.Configure();
        }
        /// <summary>
        /// Initializes a new instance of <c>NHibernateApplicationConfiguration</c> class.
        /// </summary>
        /// <param name="fileName">The filename of the external XML source.</param>
        public NHibernateApplicationConfiguration(string fileName)
            : base()
        {
            this.Configure(fileName);
        }
        #endregion
    }
}
