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

namespace Apworks.Config.Fluent
{
    /// <summary>
    /// Represents that the implemented classes are message handler configurators.
    /// </summary>
    public interface IHandlerConfigurator : IConfigSourceConfigurator { }

    /// <summary>
    /// Represents the message handler configurator.
    /// </summary>
    public class HandlerConfigurator : ConfigSourceConfigurator, IHandlerConfigurator
    {
        #region Private Fields
        private readonly string name;
        private readonly HandlerKind handlerKind;
        private readonly HandlerSourceType sourceType;
        private readonly string source;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>HandlerConfigurator</c> class.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="name">The name of the message handler.</param>
        /// <param name="handlerKind">The <see cref="HandlerKind"/> which specifies the kind of the handler, can either be a Command or an Event.</param>
        /// <param name="sourceType">The <see cref="HandlerSourceType"/> which specifies the type of the source, can either be an Assembly or a Type.</param>
        /// <param name="source">The source name, if <paramref name="sourceType"/> is Assembly, the source name should be the assembly full name, if
        /// <paramref name="sourceType"/> is Type, the source name should be the assembly qualified name of the type.</param>
        public HandlerConfigurator(IConfigSourceConfigurator context, string name, HandlerKind handlerKind,
            HandlerSourceType sourceType, string source)
            : base(context)
        {
            this.name = name;
            this.handlerKind = handlerKind;
            this.sourceType = sourceType;
            this.source = source;
        }
        /// <summary>
        /// Initializes a new instance of <c>HandlerConfigurator</c> class.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="handlerKind">The <see cref="HandlerKind"/> which specifies the kind of the handler, can either be a Command or an Event.</param>
        /// <param name="sourceType">The <see cref="HandlerSourceType"/> which specifies the type of the source, can either be an Assembly or a Type.</param>
        /// <param name="source">The source name, if <paramref name="sourceType"/> is Assembly, the source name should be the assembly full name, if
        /// <paramref name="sourceType"/> is Type, the source name should be the assembly qualified name of the type.</param>
        public HandlerConfigurator(IConfigSourceConfigurator context, HandlerKind handlerKind,
            HandlerSourceType sourceType, string source)
            : this(context, Guid.NewGuid().ToString(), handlerKind, sourceType, source) { }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="container">The configuration container.</param>
        /// <returns>The configured container.</returns>
        protected override RegularConfigSource DoConfigure(RegularConfigSource container)
        {
            container.AddHandler(this.name, this.handlerKind, this.sourceType, this.source);
            return container;
        }
        #endregion
    }
}
