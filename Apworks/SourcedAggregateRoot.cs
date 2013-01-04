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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Apworks.Events;
using Apworks.Generators;
using Apworks.Snapshots;

namespace Apworks
{
    /// <summary>
    /// Represents the base class for aggregate roots that support the event sourcing mechanism.
    /// </summary>
    public abstract class SourcedAggregateRoot : ISourcedAggregateRoot
    {
        #region Private Fields
        private Guid id;
        private long version;
        private long eventVersion;
        private long branch;
        private readonly List<IDomainEvent> uncommittedEvents = new List<IDomainEvent>();
        private readonly Dictionary<Type, List<IDomainEventHandler>> domainEventHandlers = new Dictionary<Type, List<IDomainEventHandler>>();
        #endregion

        #region Internal Constants
        /// <summary>
        /// The name of the method that updates the aggregate root version
        /// and clears the uncommitted events.
        /// </summary>
        internal const string UpdateVersionAndClearUncommittedEventsMethodName = @"UpdateVersionAndClearUncommittedEvents";
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>SourcedAggregateRoot</c> class.
        /// </summary>
        public SourcedAggregateRoot()
            : this((Guid)IdentityGenerator.Instance.Generate())
        {

        }
        /// <summary>
        /// Initializes a new instance of <c>SourcedAggregateRoot</c> class.
        /// </summary>
        /// <param name="id">The unique identifier of the aggregate root.</param>
        public SourcedAggregateRoot(Guid id)
        {
            this.id = id;
            this.version = Constants.ApplicationRuntime.DefaultVersion;
            this.eventVersion = this.version;
            this.branch = Constants.ApplicationRuntime.DefaultBranch;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets all the valid domain event handlers for the specific domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event on which the handlers should be retrieved.</param>
        /// <returns>The domain event handlers.</returns>
        private IEnumerable<IDomainEventHandler> GetDomainEventHandlers(IDomainEvent domainEvent)
        {
            Type eventType = domainEvent.GetType();
            if (domainEventHandlers.ContainsKey(eventType))
                return domainEventHandlers[eventType];
            else
            {
                List<IDomainEventHandler> handlers = new List<IDomainEventHandler>();
                // firstly create and add all the handler methods defined within the aggregation root.
                MethodInfo[] allMethods = this.GetType().GetMethods(BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var handlerMethods = from method in allMethods
                                     let returnType = method.ReturnType
                                     let @params = method.GetParameters()
                                     let handlerAttributes = method.GetCustomAttributes(typeof(HandlesAttribute), false)
                                     where returnType == typeof(void) &&
                                     @params != null &&
                                     @params.Count() > 0 &&
                                     @params[0].ParameterType.Equals(eventType) &&
                                     handlerAttributes != null &&
                                     ((HandlesAttribute)handlerAttributes[0]).DomainEventType.Equals(eventType)
                                     select new { MethodInfo = method };
                foreach (var handlerMethod in handlerMethods)
                {
                    handlers.Add(new InlineDomainEventHandler(this, handlerMethod.MethodInfo));
                }
                // then read all the registered handlers.

                domainEventHandlers.Add(eventType, handlers);
                return handlers;
            }
        }
        /// <summary>
        /// Handles the given domain event on the aggregate root.
        /// </summary>
        /// <typeparam name="TEvent">The type of the domain event.</typeparam>
        /// <param name="event">The domain event which needs to be handled by aggregate root.</param>
        private void HandleEvent<TEvent>(TEvent @event)
            where TEvent : IDomainEvent
        {
            var handlers = this.GetDomainEventHandlers(@event);
            foreach (var handler in handlers)
            {
                if (handler.CanHandle(@event))
                    handler.Handle(@event);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Raises a domain event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the domain event.</typeparam>
        /// <param name="event">The domain event to be raised.</param>
        protected virtual void RaiseEvent<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            @event.ID = (Guid)IdentityGenerator.Instance.Generate();
            @event.Version = ++eventVersion;
            @event.Source = this;
            @event.AssemblyQualifiedEventType = typeof(TEvent).AssemblyQualifiedName;
            @event.Branch = Constants.ApplicationRuntime.DefaultBranch;
            @event.Timestamp = DateTime.UtcNow;
            this.HandleEvent<TEvent>(@event);
            uncommittedEvents.Add(@event);
        }
        /// <summary>
        /// When overrided in the derived classes, builds the aggregate from the given snapshot instance.
        /// </summary>
        /// <param name="snapshot">The <see cref="Apworks.Snapshots.ISnapshot"/> instance
        /// with which the aggregate is built.</param>
        protected abstract void DoBuildFromSnapshot(ISnapshot snapshot);
        /// <summary>
        /// When overrided in the derived classes, creates the snapshot instance based on the current aggregate.
        /// </summary>
        /// <returns>The <see cref="Apworks.Snapshots.ISnapshot"/> instance that is created based on
        /// the current aggregate.</returns>
        protected abstract ISnapshot DoCreateSnapshot();
        /// <summary>
        /// Updates the version of the aggregate root and clears all uncommitted events.
        /// </summary>
        /// <remarks>This method is handled by the Apworks framework internally and should not be referenced in any circumstances.</remarks>
        [Obsolete(@"This method is handled by the Apworks framework internally 
and should not be referenced in any circumstances.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void UpdateVersionAndClearUncommittedEvents()
        {
            this.version = Version;
            this.uncommittedEvents.Clear();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the hash code for current aggregate root.
        /// </summary>
        /// <returns>The calculated hash code for the current aggregate root.</returns>
        public override int GetHashCode()
        {
            return Utils.GetHashCode(this.ID.GetHashCode(),
                this.UncommittedEvents.GetHashCode(),
                this.Version.GetHashCode(),
                this.Branch.GetHashCode());
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Apworks.ISourcedAggregateRoot"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            SourcedAggregateRoot sourcedAggregateRoot = obj as SourcedAggregateRoot;
            if ((object)sourcedAggregateRoot == null)
                return false;
            return this.Equals((IEntity)sourcedAggregateRoot);
        }
        #endregion

        #region ISourcedAggregateRoot Members
        /// <summary>
        /// Builds the aggreate from the historial events.
        /// </summary>
        /// <param name="historicalEvents">The historical events from which the aggregate is built.</param>
        public virtual void BuildFromHistory(IEnumerable<IDomainEvent> historicalEvents)
        {
            if (this.uncommittedEvents.Count() > 0)
                this.uncommittedEvents.Clear();
            foreach (IDomainEvent de in historicalEvents)
                this.HandleEvent<IDomainEvent>(de);
            this.version = historicalEvents.Last().Version;
            this.eventVersion = this.version;
        }
        /// <summary>
        /// Gets all the uncommitted events.
        /// </summary>
        public virtual IEnumerable<IDomainEvent> UncommittedEvents
        {
            get { return uncommittedEvents; }
        }
        /// <summary>
        /// Gets the version of the aggregate.
        /// </summary>
        public virtual long Version
        {
            get { return this.version + uncommittedEvents.Count; }
        }
        /// <summary>
        /// Gets the branch on which the aggregate exists.
        /// </summary>
        public virtual long Branch
        {
            get { return this.branch; }
        }
        #endregion

        #region IEntity Members
        /// <summary>
        /// Gets or sets the identifier of the aggregate root.
        /// </summary>
        public virtual Guid ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        #endregion

        #region IOrignator Members
        /// <summary>
        /// Builds the aggregate from the specified snapshot instance.
        /// </summary>
        /// <param name="snapshot">The <see cref="Apworks.Snapshots.ISnapshot"/> instance
        /// with which the aggregate is built.</param>
        public virtual void BuildFromSnapshot(ISnapshot snapshot)
        {
            this.branch = snapshot.Branch;
            this.version = snapshot.Version;
            this.id = snapshot.AggregateRootID;
            DoBuildFromSnapshot(snapshot);
            this.uncommittedEvents.Clear();
        }
        /// <summary>
        /// Creates the snapshot instance based on the current aggregate.
        /// </summary>
        /// <returns>The <see cref="Apworks.Snapshots.ISnapshot"/> instance that is created based on
        /// the current aggregate.</returns>
        public virtual ISnapshot CreateSnapshot()
        {
            ISnapshot snapshot = this.DoCreateSnapshot();
            snapshot.Branch = this.Branch;
            snapshot.Version = this.Version;
            snapshot.Timestamp = DateTime.UtcNow;
            snapshot.AggregateRootID = this.id;
            return snapshot;
        }
        #endregion

        #region IEquatable<IEntity> Members
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// entity.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Apworks.ISourcedAggregateRoot"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public virtual bool Equals(IEntity other)
        {
            if (object.ReferenceEquals(this, other))
                return true;
            if ((object)other == null)
                return false;
            if (!(other is SourcedAggregateRoot))
                return false;
            SourcedAggregateRoot otherAggregateRoot = other as SourcedAggregateRoot;
            return this.ID.Equals(otherAggregateRoot.ID);
        }

        #endregion

    }
}
