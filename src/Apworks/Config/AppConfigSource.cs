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

using System.Configuration;
using System.Linq;

namespace Apworks.Config
{
    /// <summary>
    /// Represents the configuration source that uses the application/web configuration file.
    /// </summary>
    public class AppConfigSource : IConfigSource
    {
        #region Private Fields
        private ApworksConfigSection config;
        #endregion

        #region Public Constants
        /// <summary>
        /// Represents the default name of the configuration section used by Apworks framework.
        /// </summary>
        public const string DefaultConfigSection = "apworks";
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>AppConfigSource</c> class.
        /// </summary>
        public AppConfigSource()
        {
            string configSection = DefaultConfigSection;
            try
            {
                object[] apworksConfigAttributes = typeof(ApworksConfigSection).GetCustomAttributes(false);
                if (apworksConfigAttributes.Any(p => p.GetType().Equals(typeof(System.Xml.Serialization.XmlRootAttribute))))
                {
                    System.Xml.Serialization.XmlRootAttribute xmlRootAttribute = (System.Xml.Serialization.XmlRootAttribute)
                        apworksConfigAttributes.SingleOrDefault(p => p.GetType().Equals(typeof(System.Xml.Serialization.XmlRootAttribute)));
                    if (!string.IsNullOrEmpty(xmlRootAttribute.ElementName) &&
                        !string.IsNullOrWhiteSpace(xmlRootAttribute.ElementName))
                    {
                        configSection = xmlRootAttribute.ElementName;
                    }
                }
            }
            catch // If any exception occurs, suppress the exception to uuse the default config section.
            {
            }
            LoadConfig(configSection);
        }
        /// <summary>
        /// Initializes a new instance of <c>AppConfigSource</c> class.
        /// </summary>
        /// <param name="configSectionName">The name of the Configuration Section.</param>
        public AppConfigSource(string configSectionName)
        {
            LoadConfig(configSectionName);
        }
        #endregion

        #region Private Methods
        private void LoadConfig(string configSection)
        {
            this.config = (ApworksConfigSection)ConfigurationManager.GetSection(configSection);
        }
        #endregion

        #region IConfigSource Members
        /// <summary>
        /// Gets the instance of <see cref="Apworks.Config.ApworksConfigSection"/> class.
        /// </summary>
        public ApworksConfigSection Config
        {
            get { return this.config; }
        }

        #endregion
    }
}
