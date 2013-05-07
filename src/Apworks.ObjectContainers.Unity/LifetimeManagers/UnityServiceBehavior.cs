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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Apworks.ObjectContainers.Unity.LifetimeManagers
{
    /// <summary>
    /// Configures the instance provider to use the <see cref="UnityInstanceProvider"/> for service creation.
    /// </summary>
    public class UnityServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceBehavior"/> class. 
        /// </summary>
        public UnityServiceBehavior()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public string ContainerName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="System.ServiceModel.OperationContext"/> support is enabled.
        /// </summary>
        public bool OperationContextEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="System.ServiceModel.InstanceContext"/> support is enabled.
        /// </summary>
        public bool InstanceContextEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="System.ServiceModel.ServiceHostBase"/> support is enabled.
        /// </summary>
        public bool ServiceHostBaseEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="System.ServiceModel.IContextChannel"/> support is enabled.
        /// </summary>
        public bool ContextChannelEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        /// <remarks>Not used in this behavior.</remarks>
        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        /// <remarks>Updates the endpoints instance providers to use the <see cref="UnityInstanceProvider"/>.</remarks>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
            {
                throw new ArgumentNullException("serviceDescription");
            }

            if (serviceHostBase == null)
            {
                throw new ArgumentNullException("serviceHostBase");
            }

            if (this.ServiceHostBaseEnabled)
            {
                serviceHostBase.Extensions.Add(new UnityServiceHostBaseExtension());

                // We need to subscribe to the Closing event so we can remove the extension.
                serviceHostBase.Closing += new System.EventHandler(this.ServiceHostBaseClosing);
            }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.InstanceProvider =
                            new UnityInstanceProvider(serviceDescription.ServiceType, this.ContainerName);

                        if (this.OperationContextEnabled)
                        {
                            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new UnityOperationContextMessageInspector());
                        }

                        if (this.InstanceContextEnabled)
                        {
                            endpointDispatcher.DispatchRuntime.InstanceContextInitializers.Add(new UnityInstanceContextInitializer());
                        }

                        if (this.ContextChannelEnabled)
                        {
                            foreach (DispatchOperation operation in endpointDispatcher.DispatchRuntime.Operations)
                            {
                                operation.CallContextInitializers.Add(new UnityCallContextInitializer());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        /// <remarks>Not used in this behavior.</remarks>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Occurs when a communication object transitions into the closing state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs" /> that contains no event data.</param>
        private void ServiceHostBaseClosing(object sender, System.EventArgs e)
        {
            ServiceHostBase serviceHostBase = sender as ServiceHostBase;
            if (serviceHostBase != null)
            {
                serviceHostBase.Closing -= new System.EventHandler(this.ServiceHostBaseClosing);

                // We have to get this manually, as the operation context has been disposed by now.
                UnityServiceHostBaseExtension extension = serviceHostBase.Extensions.Find<UnityServiceHostBaseExtension>();
                if (extension != null)
                {
                    serviceHostBase.Extensions.Remove(extension);
                }
            }
        }
    }
}
