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
// Copyright (C) 2010-2015 by daxnet.
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

using System.Transactions;
using Apworks.Bus;
using Apworks.Snapshots;
using Apworks.Specifications;
using Apworks.Storage;
using System;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents the domain repository that uses the snapshots to perform
    /// repository operations and publishes the domain events to the specified
    /// event bus.
    /// </summary>
    public class SnapshotDomainRepository : EventPublisherDomainRepository
    {
        #region Private Fields
        private readonly IStorage storage;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SnapshotDomainRepository</c> class.
        /// </summary>
        /// <param name="storage">The <see cref="Apworks.Storage.IStorage"/> instance that is used
        /// by the current domain repository to manipulate snapshot data.</param>
        /// <param name="eventBus">The <see cref="Apworks.Bus.IEventBus"/> instance to which
        /// the domain events are published.</param>
        public SnapshotDomainRepository(IStorage storage, IEventBus eventBus)
            : base(eventBus)
        {
            this.storage = storage;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Commits the changes registered in the domain repository.
        /// </summary>
        protected override void DoCommit()
        {
            foreach (ISourcedAggregateRoot aggregateRoot in this.SaveHash)
            {
                SnapshotDataObject snapshotDataObject = SnapshotDataObject.CreateFromAggregateRoot(aggregateRoot);
                var aggregateRootId = aggregateRoot.ID;
                var aggregateRootType = aggregateRoot.GetType().AssemblyQualifiedName;
                ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(p => p.AggregateRootID == aggregateRootId && p.AggregateRootType == aggregateRootType);
                var firstMatch = this.storage.SelectFirstOnly<SnapshotDataObject>(spec);
                if (firstMatch != null)
                    this.storage.Update<SnapshotDataObject>(new PropertyBag(snapshotDataObject), spec);
                else
                    this.storage.Insert<SnapshotDataObject>(new PropertyBag(snapshotDataObject));
                foreach (var evnt in aggregateRoot.UncommittedEvents)
                {
                    this.EventBus.Publish(evnt);
                }
            }
            if (this.DistributedTransactionSupported)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    this.storage.Commit();
                    this.EventBus.Commit();
                    ts.Complete();
                }
            }
            else
            {
                this.storage.Commit();
                this.EventBus.Commit();
            }
        }
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.Committed)
                {
                    try
                    {
                        this.Commit();
                    }
                    catch
                    {
                        this.Rollback();
                        throw;
                    }
                }
                this.storage.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="Apworks.Storage.IStorage"/> instance that is used
        /// by the current domain repository to manipulate snapshot data.
        /// </summary>
        public IStorage Storage
        {
            get { return this.storage; }
        }
        #endregion

        #region IDomainRepository Members
        /// <summary>
        /// Gets the instance of the aggregate root with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>The instance of the aggregate root with the specified identifier.</returns>
        public override TAggregateRoot Get<TAggregateRoot>(Guid id)
        {
            string aggregateRootType = typeof(TAggregateRoot).AssemblyQualifiedName;
            ISpecification<SnapshotDataObject> spec = Specification<SnapshotDataObject>.Eval(p => p.AggregateRootID == id && p.AggregateRootType == aggregateRootType);
            SnapshotDataObject snapshotDataObject = this.storage.SelectFirstOnly<SnapshotDataObject>(spec);
            if (snapshotDataObject == null)
                throw new RepositoryException("The aggregate (id={0}) cannot be found in the domain repository.", id);
            ISnapshot snapshot = snapshotDataObject.ExtractSnapshot();
            TAggregateRoot aggregateRoot = this.CreateAggregateRootInstance<TAggregateRoot>();
            aggregateRoot.BuildFromSnapshot(snapshot);
            return aggregateRoot;
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public override bool DistributedTransactionSupported
        {
            get { return this.storage.DistributedTransactionSupported && base.DistributedTransactionSupported; }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            base.Rollback();
            this.storage.Rollback();
        }
        #endregion
    }
}
