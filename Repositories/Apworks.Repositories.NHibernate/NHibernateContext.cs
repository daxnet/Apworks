// ==================================================================================================================                                                                                          
//        ,::i                                                           BBB                
//       BBBBBi                                                         EBBB                
//      MBBNBBU                                                         BBB,                
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2010-2011 apworks.codeplex.com.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using Apworks.Interception;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the repository context which supports NHibernate implementation.
    /// </summary>
    [AdditionalInterfaceToProxy(typeof(INHibernateContext))]
    public class NHibernateContext : RepositoryContext, INHibernateContext
    {
        #region Private Fields
        private readonly DatabaseSessionFactory databaseSessionFactory;
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
