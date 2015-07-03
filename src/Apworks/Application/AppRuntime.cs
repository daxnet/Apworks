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

using Apworks.Config;
using System;

namespace Apworks.Application
{
    /// <summary>
    /// Represents the Application Runtime from where the application is created, initialized and started. 
    /// </summary>
    public sealed class AppRuntime
    {
        #region Private Static Fields

        private static readonly AppRuntime instance = new AppRuntime();
        private static readonly object lockObj = new object();

        #endregion Private Static Fields

        #region Private Fields

        private IApp currentApplication = null;

        #endregion Private Fields

        #region Ctor

        static AppRuntime()
        {
        }

        private AppRuntime()
        {
        }

        #endregion Ctor

        #region Public Static Properties

        /// <summary>
        /// Gets the instance of the current <c> ApplicationRuntime </c> class. 
        /// </summary>
        public static AppRuntime Instance
        {
            get { return instance; }
        }

        #endregion Public Static Properties

        #region Public Properties

        /// <summary>
        /// Gets the instance of the currently running application. 
        /// </summary>
        public IApp CurrentApplication
        {
            get { return currentApplication; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates and initializes a new application instance. 
        /// </summary>
        /// <param name="configSource">
        /// The <see cref="Apworks.Config.IConfigSource" /> instance that is used for initializing
        /// the application.
        /// </param>
        /// <returns> The initialized application instance. </returns>
        public static IApp Create(IConfigSource configSource)
        {
            lock (lockObj)
            {
                if (instance.currentApplication == null)
                {
                    lock (lockObj)
                    {
                        if (configSource.Config == null ||
                            configSource.Config.Application == null)
                            throw new ConfigException("Either apworks configuration or apworks application configuration has not been initialized in the ConfigSource instance.");
                        string typeName = configSource.Config.Application.Provider;
                        if (string.IsNullOrEmpty(typeName))
                            throw new ConfigException("The provider type has not been defined in the ConfigSource.");
                        Type type = Type.GetType(typeName);
                        if (type == null)
                            throw new InfrastructureException("The application provider defined by type '{0}' doesn't exist.", typeName);
                        instance.currentApplication = (IApp)Activator.CreateInstance(type, new object[] { configSource });
                    }
                }
            }
            return instance.currentApplication;
        }

        #endregion Public Methods
    }
}