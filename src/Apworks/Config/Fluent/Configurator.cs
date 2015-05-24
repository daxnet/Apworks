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


using System.ComponentModel;

namespace Apworks.Config.Fluent
{
    /// <summary>
    /// Represents that the implemented classes are configuration configurators.
    /// </summary>
    /// <typeparam name="TContainer">The type of the object container.</typeparam>
    public interface IConfigurator<TContainer>
    {
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <returns>The configured container.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TContainer Configure();
    }

    /// <summary>
    /// Represents the base class for all the configuration configurators.
    /// </summary>
    /// <typeparam name="TContainer">The type of the object container.</typeparam>
    public abstract class Configurator<TContainer> : IConfigurator<TContainer>
    {
        #region Private Fields
        private readonly IConfigurator<TContainer> context;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Configurator{TContainer}</c> class.
        /// </summary>
        /// <param name="context">The <see cref="IConfigurator{TContainer}"/> instance.</param>
        /// <remarks>The <paramref name="context"/> parameter specifies the configuration context
        /// which was provided by the previous configuration step and will be configured in the
        /// current step.</remarks>
        public Configurator(IConfigurator<TContainer> context)
        {
            this.context = context;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the configuration context instance.
        /// </summary>
        public IConfigurator<TContainer> Context
        {
            get { return this.context; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="container">The object container to be configured.</param>
        /// <returns>The configured container.</returns>
        protected abstract TContainer DoConfigure(TContainer container);
        #endregion

        #region IConfigurator<TContainer> Members
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <returns>The configured container.</returns>
        public TContainer Configure()
        {
            var container = this.context.Configure();
            return DoConfigure(container);
        }

        #endregion
    }
}
