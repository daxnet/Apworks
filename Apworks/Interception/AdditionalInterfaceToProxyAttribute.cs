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

namespace Apworks.Interception
{
    /// <summary>
    /// Represents that the decorated classes are requiring additional interfaces
    /// to be intercepted when Castle Dynamic Proxy is creating the proxy objects
    /// for these classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class AdditionalInterfaceToProxyAttribute : System.Attribute
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the type of the interface that needs to be intercepted
        /// when the proxy object is created.
        /// </summary>
        public Type InterfaceType { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>AdditionalInterfaceToProxyAttribute</c>.
        /// </summary>
        /// <param name="intfType">The type of the interface that needs to be intercepted
        /// when the proxy object is create.</param>
        public AdditionalInterfaceToProxyAttribute(Type intfType)
        {
            this.InterfaceType = intfType;
        }
        #endregion
    }
}
