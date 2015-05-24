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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the base class for all domain events.
    /// </summary>
    [Serializable]
    public abstract class DomainEvent : IDomainEvent
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>DomainEvent</c> class.
        /// </summary>
        public DomainEvent() { }
        /// <summary>
        /// Initializes a new instace of <c>DomainEvent</c> class.
        /// </summary>
        /// <param name="source">The source entity which raises the domain event.</param>
        public DomainEvent(IEntity source)
        {
            this.Source = source;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the hash code for current domain event.
        /// </summary>
        /// <returns>The calculated hash code for the current domain event.</returns>
        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.Source.GetHashCode(),
                this.Branch.GetHashCode(),
                this.ID.GetHashCode(),
                this.Timestamp.GetHashCode(),
                this.Version.GetHashCode());
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// entity.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Apworks.ISourcedAggregateRoot"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            DomainEvent other = obj as DomainEvent;
            if ((object)other == (object)null)
                return false;
            return this.ID == other.ID;
        }
        #endregion

        #region IDomainEvent Members
        /// <summary>
        /// Gets or sets the source entity from which the domain event was generated.
        /// </summary>
        [XmlIgnore]
        [SoapIgnore]
        [IgnoreDataMember]
        public IEntity Source
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the version of the domain event.
        /// </summary>
        public virtual long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch on which the current version of domain event exists.
        /// </summary>
        public virtual long Branch { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified type name of the event.
        /// </summary>
        public virtual string AssemblyQualifiedEventType { get; set; }
        #endregion

        #region IEvent Members
        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        public virtual DateTime Timestamp { get; set; }
        #endregion

        #region IEntity Members
        /// <summary>
        /// Gets or sets the identifier of the domain event.
        /// </summary>
        public virtual Guid ID { get; set; }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Publishes the domain event to the registered domain event handlers.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event to be published.</typeparam>
        /// <param name="domainEvent">The domain event to be published.</param>
        /// <remarks>
        /// This method publishes domain events to the domain event handlers that have been registered 
        /// to the object container. The method will use the <see cref="ServiceLocator"/> instance to
        /// resolve all the registered domain event handlers, then publish the given domain event to
        /// all of these registered handlers. The domain event handler should implement the interface
        /// <see cref="IDomainEventHandler{T}"/>.
        /// </remarks>
        public static void Publish<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent
        {
            IEnumerable<IDomainEventHandler<TDomainEvent>> handlers = ServiceLocator
                .Instance
                .ResolveAll<IDomainEventHandler<TDomainEvent>>();
            foreach (var handler in handlers)
            {
                if (handler.GetType().IsDefined(typeof(ParallelExecutionAttribute), false))
                    Task.Factory.StartNew(() => handler.Handle(domainEvent));
                else
                    handler.Handle(domainEvent);
            }
        }
        /// <summary>
        /// Publishes the domain event to the registered domain event handlers.
        /// </summary>
        /// <typeparam name="TDomainEvent">The type of the domain event to be published.</typeparam>
        /// <param name="domainEvent">The domain event to be published.</param>
        /// <param name="callback">The callback function which will be executed after the
        /// domain event has been published and processed.</param>
        /// <param name="timeout">If a domain event handler is decorated by <see cref="ParallelExecutionAttribute"/> attribute, this parameter
        /// is to specify the timeout value for the handler to process the event.</param>
        /// <remarks>
        /// This method publishes domain events to the domain event handlers that have been registered 
        /// to the object container. The method will use the <see cref="ServiceLocator"/> instance to
        /// resolve all the registered domain event handlers, then publish the given domain event to
        /// all of these registered handlers. The domain event handler should implement the interface
        /// <see cref="IDomainEventHandler{T}"/>.
        /// </remarks>
        public static void Publish<TDomainEvent>(TDomainEvent domainEvent, Action<TDomainEvent, bool, Exception> callback, TimeSpan? timeout = null)
            where TDomainEvent : class, IDomainEvent
        {
            IEnumerable<IDomainEventHandler<TDomainEvent>> handlers = ServiceLocator
                .Instance
                .ResolveAll<IDomainEventHandler<TDomainEvent>>();
            if (handlers != null && handlers.Count() > 0)
            {
                List<Task> tasks = new List<Task>();
                try
                {
                    foreach (var handler in handlers)
                    {
                        if (handler.GetType().IsDefined(typeof(ParallelExecutionAttribute), false))
                        {
                            tasks.Add(Task.Factory.StartNew(() => handler.Handle(domainEvent)));
                        }
                        else
                            handler.Handle(domainEvent);
                    }
                    if (tasks.Count > 0)
                    {
                        if (timeout == null)
                            Task.WaitAll(tasks.ToArray());
                        else
                            Task.WaitAll(tasks.ToArray(), timeout.Value);
                    }
                    callback(domainEvent, true, null);
                }
                catch (Exception ex)
                {
                    callback(domainEvent, false, ex);
                }
            }
            else
                callback(domainEvent, false, null);
        }
        #endregion
    }
}
