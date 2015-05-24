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

/* Announcement: The Unity WCF Lifetime Manager is implemented by using the source code provided by Andrew Oakley on
 * his blog post: http://blogs.msdn.com/b/atoakley/archive/2010/12/29/unity-lifetime-managers-and-wcf.aspx.
 * When using this lifetime manager, a unity extension should be installed by adding the behavior extension. For example:
 * 
 * <system.serviceModel>
    <extensions>
      <behaviorExtensions>
        <add name="unity" type="Apworks.ObjectContainers.Unity.LifetimeManagers.UnityBehaviorExtensionElement, Apworks.ObjectContainers.Unity" />
      </behaviorExtensions>
    </extensions>
  </system.serviceModel>
 * 
 * Then, specify this behavior on the service behavior:
 * 
 * <system.serviceModel>
    <extensions>
      <behaviorExtensions>
        <add name="unity" type="Apworks.ObjectContainers.Unity.LifetimeManagers.UnityBehaviorExtensionElement, Apworks.ObjectContainers.Unity" />
      </behaviorExtensions>
    </extensions>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true" />
          <serviceDebug includeExceptionDetailInFaults="false" />
          <!-- The line below specifies the behavior settings -->
          <unity operationContextEnabled="true" instanceContextEnabled="true" contextChannelEnabled="true" serviceHostBaseEnabled="true" /> 
        </behavior>
      </serviceBehaviors>
    </behaviors>
  </system.serviceModel>
 * 
 * */

using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Apworks.ObjectContainers.Unity.LifetimeManagers
{
    /// <summary>
    /// Represents a configuration element that contains a behavior extension 
    /// which enable the user to customize service or endpoint behaviors to include
    /// the container to use when using the <see cref="UnityServiceBehavior"/>.
    /// </summary>
    public class UnityBehaviorExtensionElement : BehaviorExtensionElement
    {
        /// <summary>
        /// Name of the configuration attribute for the container name.
        /// </summary>
        private const string ContainerConfigurationPropertyName = "containerName";

        /// <summary>
        /// Name of the configuration attribute for enabling the OperationContext lifetime manager.
        /// </summary>
        private const string OperationContextEnabledPropertyName = "operationContextEnabled";

        /// <summary>
        /// Name of the configuration attribute for enabling the InstanceContext lifetime manager.
        /// </summary>
        private const string InstanceContextEnabledPropertyName = "instanceContextEnabled";

        /// <summary>
        /// Name of the configuration attribute for enabling the ServiceHostBase lifetime manager.
        /// </summary>
        private const string ServiceHostBaseEnabledPropertyName = "serviceHostBaseEnabled";

        /// <summary>
        /// Name of the configuration attribute for enabling the IContextChannel lifetime manager.
        /// </summary>
        private const string ContextChannelEnabledPropertyName = "contextChannelEnabled";

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="UnityServiceBehavior"/> type.
        /// </returns>
        public override Type BehaviorType
        {
            get { return typeof(UnityServiceBehavior); }
        }

        /// <summary>
        /// Gets or sets the container name in configuration to use when creating services.
        /// </summary>
        /// <value>The container name in configuration to use when creating services.</value>
        [ConfigurationProperty(ContainerConfigurationPropertyName, IsRequired = false)]
        public string ContainerName
        {
            get { return (string)base[ContainerConfigurationPropertyName]; }
            set { base[ContainerConfigurationPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="UnityOperationContextLifetimeManager"/> support is enabled. 
        /// </summary>
        /// <value>true to enable Unity lifetime manager support for the WCF OperationContext extension, otherwise, false.</value>
        [ConfigurationProperty(OperationContextEnabledPropertyName, IsRequired = false, DefaultValue = false)]
        public bool OperationContextEnabled
        {
            get { return (bool)base[OperationContextEnabledPropertyName]; }
            set { base[OperationContextEnabledPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="UnityInstanceContextLifetimeManager"/> support is enabled. 
        /// </summary>
        /// <value>true to enable Unity lifetime manager support for the WCF InstanceContext extension, otherwise, false.</value>
        [ConfigurationProperty(InstanceContextEnabledPropertyName, IsRequired = false, DefaultValue = false)]
        public bool InstanceContextEnabled
        {
            get { return (bool)base[InstanceContextEnabledPropertyName]; }
            set { base[InstanceContextEnabledPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="UnityServiceHostBaseLifetimeManager"/> support is enabled. 
        /// </summary>
        /// <value>true to enable Unity lifetime manager support for the WCF ServiceHostBase extension, otherwise, false.</value>
        [ConfigurationProperty(ServiceHostBaseEnabledPropertyName, IsRequired = false, DefaultValue = false)]
        public bool ServiceHostBaseEnabled
        {
            get { return (bool)base[ServiceHostBaseEnabledPropertyName]; }
            set { base[ServiceHostBaseEnabledPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="UnityContextChannelLifetimeManager"/> support is enabled. 
        /// </summary>
        /// <value>true to enable Unity lifetime manager support for the WCF IContextChannel extension, otherwise, false.</value>
        [ConfigurationProperty(ContextChannelEnabledPropertyName, IsRequired = false, DefaultValue = false)]
        public bool ContextChannelEnabled
        {
            get { return (bool)base[ContextChannelEnabledPropertyName]; }
            set { base[ContextChannelEnabledPropertyName] = value; }
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new UnityServiceBehavior()
            {
                ContainerName = this.ContainerName,
                ContextChannelEnabled = this.ContextChannelEnabled,
                InstanceContextEnabled = this.InstanceContextEnabled,
                OperationContextEnabled = this.OperationContextEnabled,
                ServiceHostBaseEnabled = this.ServiceHostBaseEnabled
            };
        }
    }
}
