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
using System.Linq;
using System.Reflection;
using Apworks.Application;
using Apworks.Config;
using Castle.DynamicProxy;

namespace Apworks.Interception
{
    /// <summary>
    /// Represents the interceptor selector.
    /// </summary>
    public sealed class InterceptorSelector : IInterceptorSelector
    {
        #region Private Methods
        private MethodInfo GetMethodInBase(Type baseType, MethodInfo thisMethod)
        {
            MethodInfo[] methods = baseType.GetMethods();
            var methodQuery = methods.Where(p =>
                {
                    var retval = p.Name == thisMethod.Name &&
                    p.IsGenericMethod == thisMethod.IsGenericMethod &&
                    ((p.GetParameters() == null && thisMethod.GetParameters() == null) || (p.GetParameters().Length == thisMethod.GetParameters().Length));
                    if (!retval)
                        return false;
                    var thisMethodParameters = thisMethod.GetParameters();
                    var pMethodParameters = p.GetParameters();
                    for (int i = 0; i < thisMethodParameters.Length; i++)
                    {
                        retval &= pMethodParameters[i].ParameterType == thisMethodParameters[i].ParameterType;
                    }
                    return retval;
                });
            if (methodQuery != null && methodQuery.Count() > 0)
                return methodQuery.Single();
            return null;
        }
        #endregion

        #region IInterceptorSelector Members
        /// <summary>
        /// Selects the interceptors for the given type and method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="method">The method.</param>
        /// <param name="interceptors">The origin interceptor collection.</param>
        /// <returns>An array of interceptors specific for the given type and method.</returns>
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            IConfigSource configSource = AppRuntime.Instance.CurrentApplication.ConfigSource;
            List<IInterceptor> selectedInterceptors = new List<IInterceptor>();

            IEnumerable<string> interceptorTypes = configSource.Config.GetInterceptorTypes(type, method);
            if (interceptorTypes == null)
            {
                if (type.BaseType != null && type.BaseType != typeof(Object))
                {
                    Type baseType = type.BaseType;
                    MethodInfo methodInfoBase = null;
                    while (baseType != null && type.BaseType != typeof(Object))
                    {
                        methodInfoBase = GetMethodInBase(baseType, method);
                        if (methodInfoBase != null)
                            break;
                        baseType = baseType.BaseType;
                    }
                    if (baseType != null && methodInfoBase != null)
                    {
                        interceptorTypes = configSource.Config.GetInterceptorTypes(baseType, methodInfoBase);
                    }
                }
                if (interceptorTypes == null)
                {
                    var intfTypes = type.GetInterfaces();
                    if (intfTypes != null && intfTypes.Count() > 0)
                    {
                        foreach (var intfType in intfTypes)
                        {
                            var methodInfoBase = GetMethodInBase(intfType, method);
                            if (methodInfoBase != null)
                                interceptorTypes = configSource.Config.GetInterceptorTypes(intfType, methodInfoBase);
                            if (interceptorTypes != null)
                                break;
                        }
                    }
                }
            }

            if (interceptorTypes != null && interceptorTypes.Count() > 0)
            {
                foreach (var interceptor in interceptors)
                {
                    if (interceptorTypes.Any(p => interceptor.GetType().AssemblyQualifiedName.Equals(p)))
                        selectedInterceptors.Add(interceptor);
                }
            }

            return selectedInterceptors.ToArray();
        }

        #endregion
    }
}
