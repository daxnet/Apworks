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

namespace Apworks.Config.Fluent
{
    /// <summary>
    /// Represents that the implemented classes are object container configurators.
    /// </summary>
    public interface IObjectContainerConfigurator : IConfigSourceConfigurator
    { }

    /// <summary>
    /// Represents the object container configurator.
    /// </summary>
    public class ObjectContainerConfigurator : TypeSpecifiedConfigSourceConfigurator, IObjectContainerConfigurator
    {
        private readonly bool initFromConfigFile = false;
        private readonly string sectionName = null;
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>ObjectContainerConfigurator</c> class.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="objectContainerType">The type of the object container to be used by the application.</param>
        /// <param name="initFromConfigFile">The <see cref="Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="sectionName">The name of the section in the config file. This value must be specified when the <paramref name="initFromConfigFile"/> parameter is set to true.</param>
        public ObjectContainerConfigurator(IConfigSourceConfigurator context, Type objectContainerType, bool initFromConfigFile, string sectionName)
            : base(context, objectContainerType)
        {
            this.initFromConfigFile = initFromConfigFile;
            this.sectionName = sectionName;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="container">The configuration container.</param>
        /// <returns>The configured container.</returns>
        protected override RegularConfigSource DoConfigure(RegularConfigSource container)
        {
            container.ObjectContainer = Type;
            container.InitObjectContainerFromConfigFile = this.initFromConfigFile;
            container.ObjectContainerSectionName = this.sectionName;
            return container;
        }
        #endregion
    }
}
