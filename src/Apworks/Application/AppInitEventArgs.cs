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
using Apworks.Config;

namespace Apworks.Application
{
    /// <summary>
    /// Represents the class that contains the event data
    /// for application initialization.
    /// </summary>
    public class AppInitEventArgs : EventArgs
    {
        #region Public Properties
        /// <summary>
        /// Gets the <see cref="Apworks.Config.IConfigSource"/> instance that was used
        /// for configuring the application.
        /// </summary>
        public IConfigSource ConfigSource { get; private set; }
        /// <summary>
        /// Gets the <see cref="Apworks.IObjectContainer"/> instance with which the application
        /// registers or resolves the object dependencies.
        /// </summary>
        public ObjectContainer ObjectContainer { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>AppInitEventArgs</c> class.
        /// </summary>
        public AppInitEventArgs()
            : this(null, null)
        { }
        /// <summary>
        /// Initializes a new instance of <c>AppInitEventArgs</c> class.
        /// </summary>
        /// <param name="configSource">The <see cref="Apworks.Config.IConfigSource"/> instance that was used
        /// for configuring the application.</param>
        /// <param name="objectContainer">The <see cref="Apworks.IObjectContainer"/> instance with which the application
        /// registers or resolves the object dependencies.</param>
        public AppInitEventArgs(IConfigSource configSource, ObjectContainer objectContainer)
        {
            this.ConfigSource = configSource;
            this.ObjectContainer = objectContainer;
        }
        #endregion
    }
}
