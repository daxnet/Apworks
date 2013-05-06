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
using System.Reflection;

namespace Apworks.Config.Fluent
{
    /// <summary>
    /// Represents that the implemented classes are interception configurators.
    /// </summary>
    public interface IInterceptionConfigurator : IConfigSourceConfigurator { }

    /// <summary>
    /// Represents the interception configurator.
    /// </summary>
    public class InterceptionConfigurator : ConfigSourceConfigurator, IInterceptionConfigurator
    {
        #region Private Fields
        private readonly Type interceptorType;
        private readonly Type contractType;
        private readonly MethodInfo interceptMethod;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>InterceptionConfigurator</c> class.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="interceptorType">The type of the interceptor to be registered.</param>
        /// <param name="contractType">The type that needs to be intercepted.</param>
        /// <param name="interceptMethod">The <see cref="MethodInfo"/> instance that needs to be intercepted.</param>
        public InterceptionConfigurator(IConfigSourceConfigurator context, Type interceptorType, Type contractType, MethodInfo interceptMethod)
            : base(context)
        {
            this.interceptorType = interceptorType;
            this.contractType = contractType;
            this.interceptMethod = interceptMethod;
        }
        /// <summary>
        /// Initializes a new instance of <c>InterceptionConfigurator</c> class.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="interceptorType">The type of the interceptor to be registered.</param>
        /// <param name="contractType">The type that needs to be intercepted.</param>
        /// <param name="interceptMethod">The name of the method that needs to be intercepted.</param>
        public InterceptionConfigurator(IConfigSourceConfigurator context, Type interceptorType, Type contractType, string interceptMethod)
            : base(context)
        {
            this.interceptorType = interceptorType;
            this.contractType = contractType;
            var method = contractType.GetMethod(interceptMethod, BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
                this.interceptMethod = method;
            else
                throw new ConfigException("The method {0} requested doesn't exist in type {1}.", interceptMethod, contractType);
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
            var name = this.interceptorType.FullName;
            container.AddInterceptor(name, this.interceptorType);
            container.AddInterceptorRef(this.contractType, this.interceptMethod, name);
            return container;
        }
        #endregion
    }
}
