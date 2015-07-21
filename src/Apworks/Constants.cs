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
// Copyright (C) 2010-2015 by daxnet
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

namespace Apworks
{
    /// <summary>
    /// Represents the utility class which provides all the constants used by Apworks framework.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Represents the utility class which provides all the constants for Apworks configuration module.
        /// </summary>
        public static class Configuration
        {
            /// <summary>
            /// The name of the configuration section which holds the configuration elements.
            /// </summary>
            public const string ConfigurationSectionName = @"apworksConfiguration";
            /// <summary>
            /// The name of the default identity generator.
            /// </summary>
            public const string DefaultIdentityGeneratorName = @"defaultIdentityGenerator";
            /// <summary>
            /// The name of the default sequence generator.
            /// </summary>
            public const string DefaultSequenceGeneratorName = @"defaultSequenceGenerator";
        }
        /// <summary>
        /// Represents the constants and readonly fields used during the running of the application.
        /// </summary>
        public static class ApplicationRuntime
        {
            /// <summary>
            /// Represents the default version number.
            /// </summary>
            public static readonly long DefaultVersion = 0;
            /// <summary>
            /// Represents the default branch number.
            /// </summary>
            public static readonly long DefaultBranch = 0;
        }
    }
}
