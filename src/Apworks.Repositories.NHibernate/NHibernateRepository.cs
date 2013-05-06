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

using Apworks.Repositories.NHibernate.Properties;
using Apworks.Specifications;
using Apworks.Storage;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Apworks.Repositories.NHibernate
{
    /// <summary>
    /// Represents the repository which supports the NHibernate implementation.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public class NHibernateRepository<TAggregateRoot> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        #region Private Fields
        //private readonly ISession session = null;
        private readonly INHibernateContext nhContext = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>NHibernateRepository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The instance of the repository context.</param>
        public NHibernateRepository(IRepositoryContext context)
            : base(context)
        {
            nhContext = context as INHibernateContext;
            if (nhContext == null)
                throw new RepositoryException(Resources.EX_INVALID_CONTEXT_TYPE);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity object to be added.</param>
        protected override void DoAdd(TAggregateRoot entity)
        {
            this.Context.RegisterNew(entity);
        }
        /// <summary>
        /// Gets the entity instance from repository by a given key.
        /// </summary>
        /// <param name="key">The key of the entity.</param>
        /// <returns>The instance of the entity.</returns>
        protected override TAggregateRoot DoGetByKey(object key)
        {
            return nhContext.GetByKey<TAggregateRoot>(key);
        }
        /// <summary>
        /// Gets all the aggregate roots that match the given specification, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enum which specifies the sort order.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder)
        {
            var results = this.DoFindAll(specification, sortPredicate, sortOrder);
            if (results == null || results.Count() == 0)
                throw new RepositoryException(Resources.EX_GETALL_FAIL);
            return results;
        }
        /// <summary>
        /// Gets all the aggregate roots that match the given specification with paging enabled, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, int pageNumber, int pageSize)
        {
            var results = this.DoFindAll(specification, sortPredicate, sortOrder, pageNumber, pageSize);
            if (results == null || results.Count() == 0)
                throw new RepositoryException(Resources.EX_GETALL_FAIL);
            return results;
        }
        /// <summary>
        /// Removes the entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        protected override void DoRemove(TAggregateRoot entity)
        {
            this.Context.RegisterDeleted(entity);
        }
        /// <summary>
        /// Updates the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        protected override void DoUpdate(TAggregateRoot entity)
        {
            this.Context.RegisterModified(entity);
        }
        /// <summary>
        /// Finds all the aggregate roots that match the given specification, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enum which specifies the sort order.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected override IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return nhContext.FindAll<TAggregateRoot>(specification, sortPredicate, sortOrder);
        }
        /// <summary>
        /// Finds all the aggregate roots that match the given specification with paging enabled, and sorts the aggregate roots
        /// by using the provided sort predicate and the specified sort order.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The number of objects per page.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <returns>All the aggregate roots that match the given specification and were sorted by using the given sort predicate and the sort order.</returns>
        protected override PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return nhContext.FindAll<TAggregateRoot>(specification, sortPredicate, sortOrder, pageNumber, pageSize);
        }
        /// <summary>
        /// Gets a single entity instance from repository by using the given specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The entity instance.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override TAggregateRoot DoGet(ISpecification<TAggregateRoot> specification)
        {
            TAggregateRoot result = this.DoFind(specification);
            if (result == null)
                throw new RepositoryException(Resources.EX_GETBYSPEC_FAIL);
            return result;
        }
        /// <summary>
        /// Finds a single aggregate root that matches the given specification.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>The instance of the aggregate root.</returns>
        protected override TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification)
        {
            return nhContext.Find<TAggregateRoot>(specification);
        }
        /// <summary>
        /// Checkes whether the aggregate root which matches the given specification exists.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <returns>True if the aggregate root exists, otherwise false.</returns>
        protected override bool DoExists(ISpecification<TAggregateRoot> specification)
        {
            return this.DoFind(specification) != null;
        }
        /// <summary>
        /// Gets all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate roots.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoGetAll(specification, sortPredicate, sortOrder);
        }
        /// <summary>
        /// Gets all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate roots.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoGetAll(specification, sortPredicate, sortOrder, pageNumber, pageSize);
        }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override IEnumerable<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoFindAll(specification, sortPredicate, sortOrder);
        }
        /// <summary>
        /// Finds all the aggregate roots from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate roots should match.</param>
        /// <param name="sortPredicate">The sort predicate which is used for sorting.</param>
        /// <param name="sortOrder">The <see cref="Apworks.Storage.SortOrder"/> enumeration which specifies the sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of objects per page.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override PagedResult<TAggregateRoot> DoFindAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, Storage.SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoFindAll(specification, sortPredicate, sortOrder, pageNumber, pageSize);
        }
        /// <summary>
        /// Gets a single aggregate root from repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        [Obsolete("The method is obsolete, use FindXXX instead.")]
        protected override TAggregateRoot DoGet(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoGet(specification, eagerLoadingProperties);
        }
        /// <summary>
        /// Finds a single aggregate root from the repository.
        /// </summary>
        /// <param name="specification">The specification with which the aggregate root should match.</param>
        /// <param name="eagerLoadingProperties">The properties for the aggregated objects that need to be loaded.</param>
        /// <returns>The aggregate root.</returns>
        protected override TAggregateRoot DoFind(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return this.DoFind(specification, eagerLoadingProperties);
        }
        #endregion
    }
}
