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

using System.Collections.Generic;
using Apworks.Events;
using Apworks.Snapshots;

namespace Apworks
{
    /// <summary>
    /// Represents that the implemented classes are aggregate roots that
    /// support event sourcing mechanism.
    /// </summary>
    public interface ISourcedAggregateRoot : IAggregateRoot, ISnapshotOrignator
    {
        /// <summary>
        /// Builds the aggreate from the historial events.
        /// </summary>
        /// <param name="historicalEvents">The historical events from which the aggregate is built.</param>
        void BuildFromHistory(IEnumerable<IDomainEvent> historicalEvents);
        /// <summary>
        /// Gets all the uncommitted events.
        /// </summary>
        IEnumerable<IDomainEvent> UncommittedEvents { get; }
        /// <summary>
        /// Gets the version of the aggregate.
        /// </summary>
        long Version { get; }
        /// <summary>
        /// Gets the branch on which the aggregate exists.
        /// </summary>
        long Branch { get; }
    }
}
