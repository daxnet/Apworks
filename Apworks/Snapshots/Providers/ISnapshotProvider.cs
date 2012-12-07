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

using System;

namespace Apworks.Snapshots.Providers
{
    /// <summary>
    /// Represents that the implemented classes are snapshot providers.
    /// </summary>
    public interface ISnapshotProvider : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value which indicates
        /// whether the snapshot should be created or updated for the given
        /// aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root.</param>
        /// <returns>True if the snapshot should be created or updated, 
        /// otherwise false.</returns>
        bool CanCreateOrUpdateSnapshot(ISourcedAggregateRoot aggregateRoot);
        /// <summary>
        /// Creates or updates the snapshot for the given aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root on which the snapshot is created or updated.</param>
        void CreateOrUpdateSnapshot(ISourcedAggregateRoot aggregateRoot);
        /// <summary>
        /// Gets the snapshot for the aggregate root with the given type and identifier.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>The snapshot instance.</returns>
        ISnapshot GetSnapshot(Type aggregateRootType, Guid id);
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value which indicates whether the snapshot
        /// exists for the aggregate root with the given type and identifier.
        /// </summary>
        /// <param name="aggregateRootType">The type of the aggregate root.</param>
        /// <param name="id">The identifier of the aggregate root.</param>
        /// <returns>True if the snapshot exists, otherwise false.</returns>
        bool HasSnapshot(Type aggregateRootType, Guid id);
        /// <summary>
        /// Gets a <see cref="Apworks.Snapshots.Providers.SnapshotProviderOption"/> value
        /// which indicates the option when using the snapshot functionalities.
        /// </summary>
        SnapshotProviderOption Option { get; }
    }
}
