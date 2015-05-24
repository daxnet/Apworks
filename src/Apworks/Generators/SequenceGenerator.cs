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
using System.Configuration;
using Apworks.Application;
using Apworks.Config;
using Apworks.Properties;

namespace Apworks.Generators
{
    /// <summary>
    /// Represents the default sequence generator.
    /// </summary>
    public sealed class SequenceGenerator : ISequenceGenerator
    {
        #region Private Fields
        private static readonly SequenceGenerator instance = new SequenceGenerator();
        private readonly ISequenceGenerator generator = null;
        #endregion

        #region Ctor
        static SequenceGenerator() { }

        private SequenceGenerator()
        {
            try
            {
                if (AppRuntime.Instance.CurrentApplication == null)
                    throw new ApworksException("The application has not been initialized and started yet.");

                if (AppRuntime.Instance.CurrentApplication.ConfigSource == null ||
                    AppRuntime.Instance.CurrentApplication.ConfigSource.Config == null ||
                    AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators == null ||
                    AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.SequenceGenerator == null ||
                    string.IsNullOrEmpty(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.SequenceGenerator.Provider) ||
                    string.IsNullOrWhiteSpace(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.SequenceGenerator.Provider))
                {
                    generator = new SequentialIdentityGenerator();
                }
                else
                {
                    Type type = Type.GetType(AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.SequenceGenerator.Provider);
                    if (type == null)
                        throw new ConfigException(string.Format("Unable to create the type from the name {0}.", AppRuntime.Instance.CurrentApplication.ConfigSource.Config.Generators.SequenceGenerator.Provider));
                    if (type.Equals(this.GetType()))
                        throw new ApworksException("Type {0} cannot be used as sequence generator, it is maintained by the Apworks framework internally.", this.GetType().AssemblyQualifiedName);

                    generator = (ISequenceGenerator)Activator.CreateInstance(type);
                }
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApworksException(Resources.EX_GET_IDENTITY_GENERATOR_FAIL, ex);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the singleton instance of <c>SequenceGenerator</c> class.
        /// </summary>
        public static SequenceGenerator Instance
        {
            get { return instance; }
        }
        #endregion

        #region ISequenceGenerator Members
        /// <summary>
        /// Gets the next value of the sequence.
        /// </summary>
        public object Next
        {
            get { return generator.Next; }
        }

        #endregion
    }
}
