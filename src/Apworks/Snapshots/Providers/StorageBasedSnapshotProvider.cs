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

using Apworks.Storage;

namespace Apworks.Snapshots.Providers
{
    /// <summary>
    /// Represents the snapshot providers that utilize both event storage and snapshot storage to
    /// implement their functionalities.
    /// </summary>
    public abstract class StorageBasedSnapshotProvider : SnapshotProvider
    {
        #region Private Fields
        private readonly IStorage eventStorage;
        private readonly IStorage snapshotStorage;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>StorageBasedSnapshotProvider</c> class.
        /// </summary>
        /// <param name="eventStorage">The instance of the event storage that is used for initializing the <c>StorageBasedSnapshotProvider</c> class.</param>
        /// <param name="snapshotStorage">The instance of the snapshot storage this is used for initializing the <c>StorageBasedSnapshotProvider</c> class.</param>
        /// <param name="option">The snapshot provider option.</param>
        public StorageBasedSnapshotProvider(IStorage eventStorage, IStorage snapshotStorage, SnapshotProviderOption option)
            : base(option)
        {
            this.eventStorage = eventStorage;
            this.snapshotStorage = snapshotStorage;
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
                this.eventStorage.Dispose();
                this.snapshotStorage.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the instance of the event storage used by <c>StorageBasedSnapshotProvider</c>.
        /// </summary>
        public IStorage EventStorage
        {
            get { return this.eventStorage; }
        }
        /// <summary>
        /// Gets the instance of the snapshot storage used by <c>StorageBasedSnapshotProvider</c>.
        /// </summary>
        public IStorage SnapshotStorage
        {
            get { return this.snapshotStorage; }
        }
        #endregion
    }
}
