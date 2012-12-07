using System;
using System.Collections.Generic;
using Apworks.Interception;
using NHibernate;
using NHibernate.Cfg;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the repository context which supports NHibernate implementation.
    /// </summary>
    [AdditionalInterfaceToProxy(typeof(INHibernateContext))]
    public class NHibernateContext : RepositoryContext, INHibernateContext
    {
        #region Private Fields
        [ThreadStatic]
        private readonly DatabaseSessionFactory databaseSessionFactory;
        [ThreadStatic]
        private readonly ISession session = null;
        private readonly object sync = new object();
        private readonly Dictionary<string, object> repositories = new Dictionary<string, object>();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>NHibernateContext</c> class.
        /// </summary>
        protected NHibernateContext()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of <c>NHibernateContext</c> class.
        /// </summary>
        public NHibernateContext(Configuration nhibernateConfig)
            : this()
        {
            databaseSessionFactory = new DatabaseSessionFactory(nhibernateConfig);
            session = databaseSessionFactory.Session;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // The dispose method will no longer be responsible for the commit
                // handling. Since the object container might handle the lifetime
                // of the repository context on the WCF per-request basis, users should
                // handle the commit logic by themselves.
                //if (!committed)
                //    Commit();
                ISession dbSession = session;
                if (dbSession != null && dbSession.IsOpen)
                {
                    dbSession.Close();
                }
                dbSession.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the instance of the NHibernate Session object.
        /// </summary>
        public ISession Session { get { return session; } }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        {
            lock (sync)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var obj in NewCollection)
                            session.Save(obj);
                        foreach (var obj in ModifiedCollection)
                            session.Update(obj);
                        foreach (var obj in DeletedCollection)
                            session.Delete(obj);
                        transaction.Commit();
                        ClearRegistrations();
                        Committed = true;
                    }
                    catch
                    {
                        if (transaction.IsActive)
                            transaction.Rollback();
                        this.session.Clear();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            Committed = false;
        }

        #endregion
        
    }
}
