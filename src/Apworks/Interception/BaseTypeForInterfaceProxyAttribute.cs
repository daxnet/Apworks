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

namespace Apworks.Interception
{
    /// <summary>
    /// Represents that the decorated classes are requiring a base type
    /// when its interface is proxied by the Castle dynamic proxy mechanism.
    /// </summary>
    /// <remarks>By using this attribute on the class, when the Castle Dynamic
    /// Proxy framework is creating the proxy class for this class' interface,
    /// the base type specified in this attribute will be used as the base type
    /// for the created proxy class.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class BaseTypeForInterfaceProxyAttribute : System.Attribute
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the base type from which the generated proxy object
        /// should derive.
        /// </summary>
        public Type BaseType { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>BaseTypeForInterfaceProxyAttribute</c> class.
        /// </summary>
        /// <param name="baseType">The base type from which the generated proxy object
        /// should derive.</param>
        public BaseTypeForInterfaceProxyAttribute(Type baseType)
        {
            this.BaseType = baseType;
        }
        #endregion
    }
}
