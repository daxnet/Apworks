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

using Apworks.ObjectContainers.Unity;

namespace Apworks.Config.Fluent
{
    /// <summary>
    /// Represents the Extension Method provider which provides additional APIs
    /// for using Unity container, based on the existing Apworks Fluent API routines.
    /// </summary>
    public static class UnityContainerFluentExtender
    {
        #region IApworksConfigurator Extensions
        /// <summary>
        /// Configures the Apworks framework by using Unity as the object container and other settings by default.
        /// </summary>
        /// <param name="configurator">The <see cref="IApworksConfigurator"/> instance to be extended.</param>
        /// <param name="containerInitFromConfigFile">The <see cref="System.Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="containerConfigSectionName">The name of the section in the config file. This value must be specified when the <paramref name="containerInitFromConfigFile"/> parameter is set to true.</param>
        /// <returns>The <see cref="IObjectContainerConfigurator"/> instace.</returns>
        public static IObjectContainerConfigurator UsingUnityContainerWithDefaultSettings(this IApworksConfigurator configurator, bool containerInitFromConfigFile = false, string containerConfigSectionName = null)
        {
            return configurator.WithDefaultSettings().UsingUnityContainer(containerInitFromConfigFile, containerConfigSectionName);
        }
        #endregion

        #region ISequenceGeneratorConfigurator Extensions
        /// <summary>
        /// Configures the Apworks framework by using Unity as the object container.
        /// </summary>
        /// <param name="configurator">The <see cref="ISequenceGeneratorConfigurator"/> instance to be extended.</param>
        /// <param name="initFromConfigFile">The <see cref="System.Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="sectionName">The name of the section in the config file. This value must be specified when the <paramref name="initFromConfigFile"/> parameter is set to true.</param>
        /// <returns>The <see cref="IObjectContainerConfigurator"/> instace.</returns>
        public static IObjectContainerConfigurator UsingUnityContainer(this ISequenceGeneratorConfigurator configurator, bool initFromConfigFile = false, string sectionName = null)
        {
            return configurator.UsingObjectContainer<UnityObjectContainer>(initFromConfigFile, sectionName);
        }
        #endregion

        #region IHandlerConfigurator Extensions
        /// <summary>
        /// Configures the Apworks framework by using Unity as the object container.
        /// </summary>
        /// <param name="configurator">The <see cref="IHandlerConfigurator"/> instance to be extended.</param>
        /// <param name="initFromConfigFile">The <see cref="System.Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="sectionName">The name of the section in the config file. This value must be specified when the <paramref name="initFromConfigFile"/> parameter is set to true.</param>
        /// <returns>The <see cref="IObjectContainerConfigurator"/> instace.</returns>
        public static IObjectContainerConfigurator UsingUnityContainer(this IHandlerConfigurator configurator, bool initFromConfigFile = false, string sectionName = null)
        {
            return configurator.UsingObjectContainer<UnityObjectContainer>(initFromConfigFile, sectionName);
        }
        #endregion

        #region IExceptionHandlerConfigurator Extensions
        /// <summary>
        /// Configures the Apworks framework by using Unity as the object container.
        /// </summary>
        /// <param name="configurator">The <see cref="IExceptionHandlerConfigurator"/> instance to be extended.</param>
        /// <param name="initFromConfigFile">The <see cref="System.Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="sectionName">The name of the section in the config file. This value must be specified when the <paramref name="initFromConfigFile"/> parameter is set to true.</param>
        /// <returns>The <see cref="IObjectContainerConfigurator"/> instace.</returns>
        public static IObjectContainerConfigurator UsingUnityContainer(this IExceptionHandlerConfigurator configurator, bool initFromConfigFile = false, string sectionName = null)
        {
            return configurator.UsingObjectContainer<UnityObjectContainer>(initFromConfigFile, sectionName);
        }
        #endregion

        #region IInterceptionConfigurator Extensions
        /// <summary>
        /// Configures the Apworks framework by using Unity as the object container.
        /// </summary>
        /// <param name="configurator">The <see cref="IInterceptionConfigurator"/> instance to be extended.</param>
        /// <param name="initFromConfigFile">The <see cref="System.Boolean"/> value which indicates whether the container configuration should be read from the config file.</param>
        /// <param name="sectionName">The name of the section in the config file. This value must be specified when the <paramref name="initFromConfigFile"/> parameter is set to true.</param>
        /// <returns>The <see cref="IObjectContainerConfigurator"/> instace.</returns>
        public static IObjectContainerConfigurator UsingUnityContainer(this IInterceptionConfigurator configurator, bool initFromConfigFile = false, string sectionName = null)
        {
            return configurator.UsingObjectContainer<UnityObjectContainer>(initFromConfigFile, sectionName);
        }
        #endregion
    }
}
