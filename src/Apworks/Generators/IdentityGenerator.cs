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
using Apworks.Application;
using Apworks.Config;

namespace Apworks.Generators
{
    /// <summary>
    /// Represents the default identity generator.
    /// </summary>
    public sealed class IdentityGenerator : IIdentityGenerator
    {
        #region Private Fields
        private static readonly IdentityGenerator instance = new IdentityGenerator();
        private readonly IIdentityGenerator generator = null;
        #endregion

        #region Ctor
        static IdentityGenerator() { }

        private IdentityGenerator()
        {
            if (AppRuntime.Instance.CurrentApplication == null)
                throw new ApworksException("The application has not been initialized and started yet.");
            if (AppRuntime.Instance.CurrentApplication.ConfigSource == null ||
                AppRuntime.Instance.CurrentApplication.ConfigSource.Config == null ||
                AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators == null ||
                AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.IdentityGenerator == null ||
                string.IsNullOrEmpty(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.IdentityGenerator.Provider) ||
                string.IsNullOrWhiteSpace(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.IdentityGenerator.Provider))
            {
                generator = new SequentialIdentityGenerator();
            }
            else
            {
                Type type = Type.GetType(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.IdentityGenerator.Provider);
                if (type == null)
                    throw new ConfigException("Unable to create the type from the name {0}.", AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.IdentityGenerator.Provider);
                if (type.Equals(this.GetType()))
                    throw new ApworksException("Type {0} cannot be used as identity generator, it is maintained by the Apworks framework internally.", this.GetType().AssemblyQualifiedName);

                generator = (IIdentityGenerator)Activator.CreateInstance(type);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the instance of the <c>IdentityGenerator</c> class.
        /// </summary>
        public static IdentityGenerator Instance
        {
            get { return instance; }
        }
        #endregion

        #region IIdentityGenerator Members
        /// <summary>
        /// Generates the identity.
        /// </summary>
        /// <returns>The generated identity instance.</returns>
        public object Generate()
        {
            return generator.Generate();
        }

        #endregion
    }
}
