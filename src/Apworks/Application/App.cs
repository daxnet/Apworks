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
using System.Collections.Generic;
using Apworks.Config;
using Castle.DynamicProxy;

namespace Apworks.Application
{
    /// <summary>
    /// Represents the implementation of the application.
    /// </summary>
    public class App : IApp
    {
        #region Private Fields
        private readonly IConfigSource configSource;
        private readonly ObjectContainer objectContainer;
        private readonly List<IInterceptor> interceptors;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Application</c> class.
        /// </summary>
        /// <param name="configSource">The <see cref="Apworks.Config.IConfigSource"/> instance that was used
        /// for configuring the application.</param>
        public App(IConfigSource configSource)
        {
            if (configSource==null)
                throw new ArgumentNullException("configSource");
            if (configSource.Config == null)
                throw new ConfigException("ApworksConfigSection has not been defined in the ConfigSource instance.");
            if (configSource.Config.ObjectContainer == null)
                throw new ConfigException("No ObjectContainer instance has been specified in the ApworksConfigSection.");
            this.configSource = configSource;    
            string objectContainerProviderName = configSource.Config.ObjectContainer.Provider;
            if (string.IsNullOrEmpty(objectContainerProviderName) ||
                string.IsNullOrWhiteSpace(objectContainerProviderName))
                throw new ConfigException("The ObjectContainer provider has not been defined in the ConfigSource.");
            Type objectContainerType = Type.GetType(objectContainerProviderName);
            if (objectContainerType == null)
                throw new InfrastructureException("The ObjectContainer defined by type {0} doesn't exist.", objectContainerProviderName);
            this.objectContainer = (ObjectContainer)Activator.CreateInstance(objectContainerType);
            if (this.configSource.Config.ObjectContainer.InitFromConfigFile)
            {
                string sectionName = this.configSource.Config.ObjectContainer.SectionName;
                if (!string.IsNullOrEmpty(sectionName) && !string.IsNullOrWhiteSpace(sectionName))
                {
                    this.objectContainer.InitializeFromConfigFile(sectionName);
                }
                else
                    throw new ConfigException("Section name for the ObjectContainer configuration should also be provided when InitFromConfigFile has been set to true.");
            }
            this.interceptors = new List<IInterceptor>();
            if (this.configSource.Config.Interception != null &&
                this.configSource.Config.Interception.Interceptors != null)
            {
                foreach (InterceptorElement interceptorElement in this.configSource.Config.Interception.Interceptors)
                {
                    Type interceptorType = Type.GetType(interceptorElement.Type);
                    if (interceptorType == null)
                        continue;
                    IInterceptor interceptor = (IInterceptor)Activator.CreateInstance(interceptorType);
                    this.interceptors.Add(interceptor);
                }
            }
        }
        #endregion

        #region Private Methods
        private void DoInitialize()
        {
            EventHandler<AppInitEventArgs> d = this.Initialize;
            if (d != null)
                d(this, new AppInitEventArgs(this.configSource, this.objectContainer));
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Starts the application
        /// </summary>
        protected virtual void OnStart() { }
        #endregion

        #region IApplication Members
        /// <summary>
        /// Gets the <see cref="Apworks.Config.IConfigSource"/> instance that was used
        /// for configuring the application.
        /// </summary>
        public IConfigSource ConfigSource
        {
            get { return this.configSource; }
        }
        /// <summary>
        /// Gets the <see cref="Apworks.IObjectContainer"/> instance with which the application
        /// registers or resolves the object dependencies.
        /// </summary>
        public ObjectContainer ObjectContainer
        {
            get { return this.objectContainer; }
        }
        /// <summary>
        /// Gets a list of <see cref="Castle.DynamicProxy.IInterceptor"/> instances that are
        /// registered on the current application.
        /// </summary>
        public IEnumerable<IInterceptor> Interceptors
        {
            get { return this.interceptors; }
        }
        /// <summary>
        /// Starts the application.
        /// </summary>
        public void Start()
        {
            this.DoInitialize();
            this.OnStart();
        }
        /// <summary>
        /// The event that occurs when the application is initializing.
        /// </summary>
        public event EventHandler<AppInitEventArgs> Initialize;

        #endregion
    }
}
