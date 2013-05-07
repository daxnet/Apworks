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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Apworks.ObjectContainers.Unity.LifetimeManagers
{
    /// <summary>
    /// Initializes and cleans up thread-local storage for the thread that invokes user code to support the <see cref="UnityContextChannelLifetimeManager"/>.
    /// </summary>
    public class UnityCallContextInitializer : ICallContextInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityCallContextInitializer"/> class.
        /// </summary>
        public UnityCallContextInitializer()
            : base()
        {
        }

        /// <summary>
        /// Cleans up the thread that invoked the operation by removing the <see cref="UnityContextChannelExtension"/>.
        /// </summary>
        /// <param name="correlationState">The correlation object returned from the BeforeInvoke method.</param>
        public void AfterInvoke(object correlationState)
        {
            // It feels wrong going through the OperationContext to get to the channel, but since it's not passed as a parameter
            // to this method, like BeforeInvoke(), we have to do it this way.  Should we return a correlation state
            // from BeforeInvoke() to get to this?
            OperationContext.Current.Channel.Extensions.Remove(UnityContextChannelExtension.Current);
        }

        /// <summary>
        /// Initializes the operation thread by adding the <see cref="UnityContextChannelExtension"/> to the WCF client channel.
        /// </summary>
        /// <param name="instanceContext">The service instance for the operation.</param>
        /// <param name="channel">The client channel.</param>
        /// <param name="message">The incoming message.</param>
        /// <returns>A correlation object passed back as a parameter of the AfterInvoke method.</returns>
        public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            channel.Extensions.Add(new UnityContextChannelExtension());
            return null;
        }
    }
}
