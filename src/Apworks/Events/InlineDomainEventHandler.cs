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
// Copyright (C) 2010-2011 apworks.codeplex.com.
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
using System.Linq;
using System.Reflection;
using Apworks.Properties;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the domain event handler that is defined within the scope of
    /// an aggregate root and handles the domain event when <c>SourcedAggregateRoot.RaiseEvent&lt;TEvent&gt;</c>
    /// is called.
    /// </summary>
    public sealed class InlineDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        #region Private Fields
        private readonly Type domainEventType;
        private readonly Func<TDomainEvent, bool> action;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>InlineDomainEventHandler</c> class.
        /// </summary>
        /// <param name="aggregateRoot">The instance of the aggregate root within which the domain event
        /// was raised and handled.</param>
        /// <param name="mi">The method which handles the domain event.</param>
        public InlineDomainEventHandler(ISourcedAggregateRoot aggregateRoot, MethodInfo mi)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters == null || parameters.Count() == 0)
            {
                throw new ArgumentException(string.Format(Resources.EX_INVALID_HANDLER_SIGNATURE, mi.Name), "mi");
            }
            domainEventType = parameters[0].ParameterType;
            action = domainEvent =>
            {
                try
                {
                    mi.Invoke(aggregateRoot, new object[] { domainEvent });
                    return true;
                }
                catch { return false; }
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == (object)null)
                return false;
            InlineDomainEventHandler<TDomainEvent> other = obj as InlineDomainEventHandler<TDomainEvent>;
            if ((object)other == (object)null)
                return false;
            return Delegate.Equals(this.action, other.action);
        }
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current System.Object.</returns>
        public override int GetHashCode()
        {
            if (this.action != null && this.domainEventType != null)
                return Utils.GetHashCode(this.action.GetHashCode(),
                    this.domainEventType.GetHashCode());
            return base.GetHashCode();
        }
        #endregion

        #region IHandler<TDomainEvent> Members
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        public bool Handle(TDomainEvent message)
        {
            return action(message);
        }

        #endregion
    }
}
