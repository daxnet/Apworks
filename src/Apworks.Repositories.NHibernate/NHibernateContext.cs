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

using Apworks.Interception;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Apworks.Specifications;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the extension method provider for IQueryable{T} interface.
    /// </summary>
    internal static class QueryableExtender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IFutureValue<TResult> ToFutureValue<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<IQueryable<TSource>, TResult>> selector)
            where TResult : struct
        {
            var provider = (INhQueryProvider)source.Provider;
            var method = ((MethodCallExpression)selector.Body).Method;
            var expression = Expression.Call(null, method, source.Expression);
            return (IFutureValue<TResult>)provider.ExecuteFuture(expression);
        }
    }

    /// <summary>
    /// Represents the repository context which supports NHibernate implementation.
    /// </summary>
    [AdditionalInterfaceToProxy(typeof(INHibernateContext))]
    public class NHibernateContext : RepositoryContext, INHibernateContext
    {
        #region Private Fields
        private readonly DatabaseSessionFactory databaseSessionFactory;
        private readonly ISession session = null;
        private ITransaction transaction;
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
            SetupTransaction();
        }
        #endregion

        #region Private Methods
        private void SetupTransaction()
        {
            if (transaction != null)
                transaction.Dispose();
            transaction = session.BeginTransaction();
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
                if (transaction != null)
                {
                    transaction.Dispose();
                    transaction = null;
                }
                ISession dbSession = session;
                if (dbSession != null /*&& dbSession.IsOpen*/)
                {
                    //dbSession.Close();
                    dbSession.Dispose();
                    dbSession = null;
                }
                //dbSession.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterNew(object obj)
        {
            session.Save(obj);
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterDeleted(object obj)
        {
            session.Delete(obj);
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterModified(object obj)
        {
            session.Update(obj);
        }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public override bool Committed
        {
            get
            {
                return transaction != null &&
                    transaction.WasCommitted;
            }
            protected set { }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        {
            transaction.Commit();
            SetupTransaction();
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            transaction.Rollback();
            SetupTransaction();
        }

        #endregion


        #region INHibernateContext Members
        /// <summary>
        /// Gets the aggregate root instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the aggregate root.</param>
        /// <returns>The instance of the aggregate root.</returns>
        public TAggregateRoot GetByKey<TAggregateRoot>(object key) where TAggregateRoot : class, IAggregateRoot
        {
            var result = (TAggregateRoot)this.session.Get(typeof(TAggregateRoot), key);
            // Use of implicit transactions is discouraged.
            // For more information please refer to: http://www.hibernatingrhinos.com/products/nhprof/learn/alert/DoNotUseImplicitTransactions
            Commit();
            return result;
        }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <returns>The aggregate roots.</returns>
        public IEnumerable<TAggregateRoot> FindAll<TAggregateRoot>(ISpecification<TAggregateRoot> specification, System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder) where TAggregateRoot : class, IAggregateRoot
        {
            List<TAggregateRoot> result = null;
            var query = this.session.Query<TAggregateRoot>()
                .Where(specification.GetExpression());
            switch (sortOrder)
            {
                case Storage.SortOrder.Ascending:
                    if (sortPredicate != null)
                        result = query.OrderBy(sortPredicate).ToList();
                    break;
                case Storage.SortOrder.Descending:
                    if (sortPredicate != null)
                        result = query.OrderByDescending(sortPredicate).ToList();
                    break;
                default:
                    result = query.ToList();
                    break;
            }
            // Use of implicit transactions is discouraged.
            // For more information please refer to: http://www.hibernatingrhinos.com/products/nhprof/learn/alert/DoNotUseImplicitTransactions
            Commit();
            return result;
        }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The number of objects per page.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>The aggregate roots.</returns>
        public PagedResult<TAggregateRoot> FindAll<TAggregateRoot>(ISpecification<TAggregateRoot> specification, System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, int pageNumber, int pageSize) where TAggregateRoot : class, IAggregateRoot
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "The pageNumber is one-based and should be larger than zero.");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "The pageSize is one-based and should be larger than zero.");
            if (sortPredicate == null)
                throw new ArgumentNullException("sortPredicate");

            var query = this.session.Query<TAggregateRoot>()
                .Where(specification.GetExpression());

            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            int totalCount = 0;
            int totalPages = 0;
            List<TAggregateRoot> pagedData = null;
            PagedResult<TAggregateRoot> result = null;

            switch (sortOrder)
            {
                case Storage.SortOrder.Ascending:
                    totalCount = query.ToFutureValue(x => x.Count()).Value;
                    totalPages = (totalCount + pageSize - 1) / pageSize;
                    pagedData = query.OrderBy(sortPredicate).Skip(skip).Take(take).ToFuture().ToList();
                    result = new PagedResult<TAggregateRoot>(totalCount, totalPages, pageSize, pageNumber, pagedData);
                    break;
                case Storage.SortOrder.Descending:
                    totalCount = query.ToFutureValue(x => x.Count()).Value;
                    totalPages = (totalCount + pageSize - 1) / pageSize;
                    pagedData = query.OrderByDescending(sortPredicate).Skip(skip).Take(take).ToFuture().ToList();
                    result = new PagedResult<TAggregateRoot>(totalCount, totalPages, pageSize, pageNumber, pagedData);
                    break;
                default:
                    break;

            }
            // Use of implicit transactions is discouraged.
            // For more information please refer to: http://www.hibernatingrhinos.com/products/nhprof/learn/alert/DoNotUseImplicitTransactions
            Commit();
            return result;
        }
        /// <summary>
        /// Finds a single aggregate root from the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        public TAggregateRoot Find<TAggregateRoot>(ISpecification<TAggregateRoot> specification) where TAggregateRoot : class, IAggregateRoot
        {
            var result = this.session.Query<TAggregateRoot>().Where(specification.GetExpression()).FirstOrDefault();
            // Use of implicit transactions is discouraged.
            // For more information please refer to: http://www.hibernatingrhinos.com/products/nhprof/learn/alert/DoNotUseImplicitTransactions
            Commit();
            return result;
        }

        #endregion
    }
}
