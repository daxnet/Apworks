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

using System;
using System.Collections.Generic;
using System.Linq;
using Apworks.Specifications;
using Apworks.Storage;

namespace Apworks.Events.Storage
{
    /// <summary>
    /// Represents the base class for domain event storages which are built based on the
    /// relational database systems.
    /// </summary>
    /// <typeparam name="TRdbmsStorage">The type of the <c>RdbmsStorage</c> which provides
    /// the required operations on the external storage mechanism.</typeparam>
    public abstract class RdbmsDomainEventStorage<TRdbmsStorage> : DisposableObject, IDomainEventStorage
        where TRdbmsStorage : RdbmsStorage
    {
        #region Private Fields
        private TRdbmsStorage storage;
        private readonly string connectionString;
        private readonly IStorageMappingResolver mappingResolver;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>RdbmsDomainEventStorage&lt;TRdbmsStorage&gt;</c> class.
        /// </summary>
        /// <param name="connectionString">The connection string which is used when connecting
        /// to the relational database system. For more information about the connection strings
        /// for different database providers, please refer to http://www.connectionstrings.com.
        /// </param>
        /// <param name="mappingResolver">The instance of the mapping resolver which resolves the table and column mappings
        /// between data objects and the relational database system.</param>
        public RdbmsDomainEventStorage(string connectionString, IStorageMappingResolver mappingResolver)
        {
            try
            {
                this.connectionString = connectionString;
                this.mappingResolver = mappingResolver;
                Type storageType = typeof(TRdbmsStorage);
                storage = (TRdbmsStorage)Activator.CreateInstance(storageType, new object[] { connectionString, mappingResolver });
            }
            catch
            {
                GC.SuppressFinalize(this);
                throw;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the connection string which is used when connecting
        /// to the relational database system. For more information about the connection strings
        /// for different database providers, please refer to http://www.connectionstrings.com.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
        }
        /// <summary>
        /// Gets the instance of the mapping resolver which resolves the table and column mappings
        /// between data objects and relational database system.
        /// </summary>
        public IStorageMappingResolver MappingResolver
        {
            get { return this.mappingResolver; }
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
            if (!this.Committed)
                this.Commit();
            storage.Dispose();
        }
        #endregion

        #region IDomainEventStorage Members
        /// <summary>
        /// Saves the specified domain event to the event storage.
        /// </summary>
        /// <param name="domainEvent">The domain event to be saved.</param>
        public void SaveEvent(IDomainEvent domainEvent)
        {
            try
            {
                DomainEventDataObject dataObject = DomainEventDataObject.FromDomainEvent(domainEvent);
                storage.Insert<DomainEventDataObject>(new PropertyBag(dataObject));
            }
            catch { throw; }
        }
        /// <summary>
        /// Loads all the domain events for the specific aggregate root from the storage.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>A list of domain events for the specific aggregate root.</returns>
        public IEnumerable<IDomainEvent> LoadEvents(Type aggregateRootType, Guid id)
        {
            try
            {
                PropertyBag sort = new PropertyBag();
                sort.AddSort<long>("Version");
                var aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
                ISpecification<DomainEventDataObject> specification = Specification<DomainEventDataObject>.Eval(p => p.SourceID == id && p.AssemblyQualifiedSourceType == aggregateRootTypeName);
                return storage.Select<DomainEventDataObject>(specification, sort, Apworks.Storage.SortOrder.Ascending).Select(p => p.ToDomainEvent());
            }
            catch { throw; }
        }
        /// <summary>
        /// Loads all the domain events for the specific aggregate root from the storage.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <param name="version">The version number.</param>
        /// <returns>A list of domain events for the specific aggregate root which occur just after
        /// the given version number.</returns>
        public IEnumerable<IDomainEvent> LoadEvents(Type aggregateRootType, Guid id, long version)
        {
            PropertyBag sort = new PropertyBag();
            sort.AddSort<long>("Version");
            var aggregateRootTypeName = aggregateRootType.AssemblyQualifiedName;
            ISpecification<DomainEventDataObject> specification = Specification<DomainEventDataObject>
                .Eval(p => p.SourceID == id && p.AssemblyQualifiedSourceType == aggregateRootTypeName && p.Version > version);
            return storage.Select<DomainEventDataObject>(specification, sort, Apworks.Storage.SortOrder.Ascending).Select(p => p.ToDomainEvent());
        }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public virtual bool DTCompatible
        {
            get
            {
                return storage.DTCompatible;
            }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public bool Committed
        {
            get { return this.storage.Committed; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public virtual void Commit()
        {
            storage.Commit();
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public virtual void Rollback()
        {
            storage.Rollback();
        }

        #endregion
    }
}
