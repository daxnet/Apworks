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
// Copyright (C) 2010-2013 apworks.org.
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

using NHibernate;
using NHibernate.Cfg;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the factory singleton for database session.
    /// </summary>
    internal sealed class DatabaseSessionFactory
    {
        #region Private Fields
        /// <summary>
        /// The session factory instance.
        /// </summary>
        private readonly ISessionFactory sessionFactory = null;
        /// <summary>
        /// The session instance.
        /// </summary>
        private ISession session = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <c>DatabaseSessionFactory</c> class.
        /// </summary>
        internal DatabaseSessionFactory()
        {
            sessionFactory = new Configuration().Configure().BuildSessionFactory();
        }
        /// <summary>
        /// Initializes a new instance of <c>DatabaseSessionFactory</c> class.
        /// </summary>
        /// <param name="nhibernateConfig">The <see cref="Configuration"/> instance used for initializing.</param>
        internal DatabaseSessionFactory(Configuration nhibernateConfig)
        {
            sessionFactory = nhibernateConfig.BuildSessionFactory();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the singleton instance of the session. If the session has not been
        /// initialized or opened, it will return a newly opened session from the session factory.
        /// </summary>
        public ISession Session
        {
            get
            {
                ISession result = session;
                if (result != null && result.IsOpen)
                    return result;
                return OpenSession();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Always opens a new session from the session factory.
        /// </summary>
        /// <returns>The newly opened session.</returns>
        public ISession OpenSession()
        {
            this.session = sessionFactory.OpenSession();
            return this.session;
        }
        #endregion

    }
}
