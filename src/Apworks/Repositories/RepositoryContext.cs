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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents the repository context.
    /// </summary>
    public abstract class RepositoryContext : DisposableObject, IRepositoryContext
    {
        #region Private Fields
        private readonly Guid id = Guid.NewGuid();
        //private readonly ThreadLocal<List<object>> localNewCollection = new ThreadLocal<List<object>>(() => new List<object>());
        //private readonly ThreadLocal<List<object>> localModifiedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        //private readonly ThreadLocal<List<object>> localDeletedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        //private readonly ThreadLocal<bool> localCommitted = new ThreadLocal<bool>(() => true);
        private readonly ConcurrentDictionary<object, byte> newCollection = new ConcurrentDictionary<object, byte>();
        private ConcurrentDictionary<object, byte> modifiedCollection = new ConcurrentDictionary<object, byte>();
        private ConcurrentDictionary<object, byte> deletedCollection = new ConcurrentDictionary<object, byte>();
        private volatile bool committed = true;
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be added to the repository.
        /// </summary>
        protected ConcurrentDictionary<object, byte> NewCollection
        {
            get { return newCollection; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be modified in the repository.
        /// </summary>
        protected ConcurrentDictionary<object, byte> ModifiedCollection
        {
            get { return modifiedCollection; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be deleted from the repository.
        /// </summary>
        protected ConcurrentDictionary<object, byte> DeletedCollection
        {
            get { return deletedCollection; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Clears all the registration in the repository context.
        /// </summary>
        /// <remarks>Note that this can only be called after the repository context has successfully committed.</remarks>
        protected void ClearRegistrations()
        {
            newCollection.Clear();
            modifiedCollection.Clear();
            deletedCollection.Clear();
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
                ClearRegistrations();
            }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Gets the ID of the repository context.
        /// </summary>
        public Guid ID
        {
            get { return id; }
        }
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterNew(object obj)
        {
            //if (localModifiedCollection.Value.Contains(obj))
            //   throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            //if (localNewCollection.Value.Contains(obj))
            //    throw new InvalidOperationException("The object has already been registered as a new object.");

            //localNewCollection.Value.Add(obj);

            newCollection.AddOrUpdate(obj, byte.MinValue, (o, b) => byte.MinValue);
            Committed = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterModified(object obj)
        {
            //if (localDeletedCollection.Value.Contains(obj))
            //    throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            //if (!localModifiedCollection.Value.Contains(obj) && !localNewCollection.Value.Contains(obj))
            //    localModifiedCollection.Value.Add(obj);

            if (deletedCollection.ContainsKey(obj))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!modifiedCollection.ContainsKey(obj) && !(newCollection.ContainsKey(obj)))
                modifiedCollection.AddOrUpdate(obj, byte.MinValue, (o, b) => byte.MinValue);

            Committed = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterDeleted(object obj)
        {
            //if (localNewCollection.Value.Contains(obj))
            //{
            //    if (localNewCollection.Value.Remove(obj))
            //        return;
            //}
            //bool removedFromModified = localModifiedCollection.Value.Remove(obj);
            //bool addedToDeleted = false;
            //if (!localDeletedCollection.Value.Contains(obj))
            //{
            //    localDeletedCollection.Value.Add(obj);
            //    addedToDeleted = true;
            //}
            //localCommitted.Value = !(removedFromModified || addedToDeleted);
            var @byte = byte.MinValue;
            if (newCollection.ContainsKey(obj))
            {
                newCollection.TryRemove(obj, out @byte);
                return;
            }
            var removedFromModified = modifiedCollection.TryRemove(obj, out @byte);
            var addedToDeleted = false;
            if (!deletedCollection.ContainsKey(obj))
            {
                deletedCollection.AddOrUpdate(obj, byte.MinValue, (o, b) => byte.MinValue);
                addedToDeleted = true;
            }
            committed = !(removedFromModified || addedToDeleted);
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public virtual bool DistributedTransactionSupported
        {
            get { return false; }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public virtual bool Committed
        {
            get { return committed; }
            protected set { committed = value; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public abstract void Commit();

        public Task CommitAsync()
        {
            return CommitAsync(CancellationToken.None);
        }

        public abstract Task CommitAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public abstract void Rollback();
        #endregion
    }
}
