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
using System.Threading.Tasks;

namespace Apworks.Events
{
    /// <summary>
    /// Represents an Event Aggregator implementation.
    /// </summary>
    /// <remarks>For more information about the Event Aggregator, please refer to: http://msdn.microsoft.com/en-us/library/ff921122(v=pandp.20).aspx
    /// </remarks>
    public class EventAggregator : IEventAggregator
    {
        #region Private Fields
        private readonly object sync = new object();
        private readonly Dictionary<Type, List<object>> eventHandlers = new Dictionary<Type, List<object>>();
        private readonly MethodInfo registerEventHandlerMethod;
        private readonly Func<object, object, bool> eventHandlerEquals = (o1, o2) =>
        {
            var o1Type = o1.GetType();
            var o2Type = o2.GetType();
            if (o1Type.IsGenericType &&
                o1Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>) &&
                o2Type.IsGenericType &&
                o2Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>))
                return o1.Equals(o2);
            return o1Type == o2Type;
        }; // checks if the two event handlers are equal. if the event handler is an action-delegated, just simply
        // compare the two with the object.Equals override (since it was overriden by comparing the two delegates. Otherwise,
        // the type of the event handler will be used because we don't need to register the same type of the event handler
        // more than once for each specific event.
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventAggregator</c> class.
        /// </summary>
        public EventAggregator()
        {
            registerEventHandlerMethod = (from p in this.GetType().GetMethods()
                                          let methodName = p.Name
                                          let parameters = p.GetParameters()
                                          where methodName == "Subscribe" &&
                                          parameters != null &&
                                          parameters.Length == 1 &&
                                          parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                                          select p).First();
        }
        /// <summary>
        /// Initializes a new instance of <c>EventAggregator</c> class.
        /// </summary>
        /// <param name="handlers">The event handlers to be registered to the Event Aggregator.</param>
        /// <remarks>
        /// All the event handlers registered to the Event Aggregator should implement the <see cref="IEventHandler{T}"/>
        /// interface, otherwise, the instance will be ignored. When using IoC containers to register dependencies,
        /// remember to specify not only the name of the dependency, but also the type of the dependency. For example,
        /// in the Unity container configuration section, you should register the handlers by using the following snippet:
        /// <code>
        /// &lt;register type="Apworks.Events.IEventAggregator, Apworks" mapTo="Apworks.Events.EventAggregator, Apworks"&gt;
        ///  &lt;constructor&gt;
        ///    &lt;param name="handlers"&gt;
        ///      &lt;array&gt;
        ///        &lt;dependency name="orderDispatchedSendEmailHandler" type="Apworks.Events.IEventHandler`1[[ByteartRetail.Domain.Events.OrderDispatchedEvent, ByteartRetail.Domain]], Apworks" /&gt;
        ///        &lt;dependency name="orderConfirmedSendEmailHandler" type="Apworks.Events.IEventHandler`1[[ByteartRetail.Domain.Events.OrderConfirmedEvent, ByteartRetail.Domain]], Apworks" /&gt;
        ///      &lt;/array&gt;
        ///    &lt;/param&gt;
        ///  &lt;/constructor&gt;
        ///&lt;/register&gt;
        /// </code>
        /// </remarks>
        public EventAggregator(object[] handlers)
            : this()
        {
            foreach (var obj in handlers)
            {
                var type = obj.GetType();
                var implementedInterfaces = type.GetInterfaces();
                foreach (var implementedInterface in implementedInterfaces)
                {
                    if (implementedInterface.IsGenericType &&
                        implementedInterface.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    {
                        var eventType = implementedInterface.GetGenericArguments().First();
                        var method = registerEventHandlerMethod.MakeGenericMethod(eventType);
                        method.Invoke(this, new object[] { obj });
                    }
                }
            }
        }
        #endregion

        #region IEventAggregator Members
        /// <summary>
        /// Subscribes the event handler to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">The event handler.</param>
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent
        {
            lock (sync)
            {
                var eventType = typeof(TEvent);
                if (eventHandlers.ContainsKey(eventType))
                {
                    var handlers = eventHandlers[eventType];
                    if (handlers != null)
                    {
                        if (!handlers.Exists(deh => eventHandlerEquals(deh, eventHandler)))
                            handlers.Add(eventHandler);
                    }
                    else
                    {
                        handlers = new List<object>();
                        handlers.Add(eventHandler);
                    }
                }
                else
                    eventHandlers.Add(eventType, new List<object> { eventHandler });
            }
        }
        /// <summary>
        /// Subscribes the event handlers to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlers">The event handlers.</param>
        public void Subscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
            where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
                Subscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// Subscribes the event handlers to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlers">The event handlers.</param>
        public void Subscribe<TEvent>(params IEventHandler<TEvent>[] eventHandlers)
            where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
                Subscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// Subscribes the <see cref="Action{T}"/> delegate to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerAction">The <see cref="Action{T}"/> delegate.</param>
        public void Subscribe<TEvent>(Action<TEvent> eventHandlerAction)
            where TEvent : class, IEvent
        {
            Subscribe<TEvent>(new ActionDelegatedEventHandler<TEvent>(eventHandlerAction));
        }
        /// <summary>
        /// Subscribes the <see cref="Action{T}"/> delegates to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerActions">The <see cref="Action{T}"/> delegates.</param>
        public void Subscribe<TEvent>(IEnumerable<Action<TEvent>> eventHandlerActions)
            where TEvent : class, IEvent
        {
            foreach (var eventHandlerAction in eventHandlerActions)
                Subscribe<TEvent>(eventHandlerAction);
        }
        /// <summary>
        /// Subscribes the <see cref="Action{T}"/> delegates to a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerActions">The <see cref="Action{T}"/> delegates.</param>
        public void Subscribe<TEvent>(params Action<TEvent>[] eventHandlerActions)
            where TEvent : class, IEvent
        {
            foreach (var eventHandlerAction in eventHandlerActions)
                Subscribe<TEvent>(eventHandlerAction);
        }
        /// <summary>
        /// Unsubscribes the event handler from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandler">The event handler.</param>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent
        {
            lock (sync)
            {
                var eventType = typeof(TEvent);
                if (eventHandlers.ContainsKey(eventType))
                {
                    var handlers = eventHandlers[eventType];
                    if (handlers != null &&
                        handlers.Exists(deh => eventHandlerEquals(deh, eventHandler)))
                    {
                        var handlerToRemove = handlers.First(deh => eventHandlerEquals(deh, eventHandler));
                        handlers.Remove(handlerToRemove);
                    }
                }
            }
        }
        /// <summary>
        /// Unsubscribes the event handlers from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlers">The event handler.</param>
        public void Unsubscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
            where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
                Unsubscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// Unsubscribes the event handlers from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlers">The event handlers.</param>
        public void Unsubscribe<TEvent>(params IEventHandler<TEvent>[] eventHandlers)
            where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
                Unsubscribe<TEvent>(eventHandler);
        }
        /// <summary>
        /// Unsubscribes the <see cref="Action{T}"/> delegate from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerAction">The <see cref="Action{T}"/> delegate.</param>
        public void Unsubscribe<TEvent>(Action<TEvent> eventHandlerAction)
            where TEvent : class, IEvent
        {
            Unsubscribe<TEvent>(new ActionDelegatedEventHandler<TEvent>(eventHandlerAction));
        }
        /// <summary>
        /// Unsubscribes the <see cref="Action{T}"/> delegates from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerActions">The <see cref="Action{T}"/> delegates.</param>
        public void Unsubscribe<TEvent>(IEnumerable<Action<TEvent>> eventHandlerActions)
            where TEvent : class, IEvent
        {
            foreach (var eventHandlerAction in eventHandlerActions)
                Unsubscribe<TEvent>(eventHandlerAction);
        }
        /// <summary>
        /// Unsubscribes the <see cref="Action{T}"/> delegates from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventHandlerActions">The <see cref="Action{T}"/> delegates.</param>
        public void Unsubscribe<TEvent>(params Action<TEvent>[] eventHandlerActions)
            where TEvent : class, IEvent
        {
            foreach (var eventHandlerAction in eventHandlerActions)
                Unsubscribe<TEvent>(eventHandlerAction);
        }
        /// <summary>
        /// Unsubscribes all the subscribed event handlers from a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        public void UnsubscribeAll<TEvent>()
            where TEvent : class, IEvent
        {
            lock (sync)
            {
                var eventType = typeof(TEvent);
                if (eventHandlers.ContainsKey(eventType))
                {
                    var handlers = eventHandlers[eventType];
                    if (handlers != null)
                        handlers.Clear();
                }
            }
        }
        /// <summary>
        /// Unsubscribes all the event handlers from the event aggregator.
        /// </summary>
        public void UnsubscribeAll()
        {
            lock (sync)
            {
                eventHandlers.Clear();
            }
        }
        /// <summary>
        /// Gets the subscribed event handlers for a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>A collection of subscribed event handlers.</returns>
        public IEnumerable<IEventHandler<TEvent>> GetSubscriptions<TEvent>()
            where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);
            if (eventHandlers.ContainsKey(eventType))
            {
                var handlers = eventHandlers[eventType];
                if (handlers != null)
                    return handlers.Select(p => p as IEventHandler<TEvent>).ToList();
                else
                    return null;
            }
            else
                return null;
        }
        /// <summary>
        /// Publishes the event to all of its subscriptions.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be published.</typeparam>
        /// <param name="event">The event to be published.</param>
        public void Publish<TEvent>(TEvent @event)
            where TEvent : class, IEvent
        {
            if (@event == null)
                throw new ArgumentNullException("evnt");
            var eventType = @event.GetType();
            if (eventHandlers.ContainsKey(eventType) &&
                eventHandlers[eventType] != null &&
                eventHandlers[eventType].Count > 0)
            {
                var handlers = eventHandlers[eventType];
                foreach (var handler in handlers)
                {
                    var eventHandler = handler as IEventHandler<TEvent>;
                    if (eventHandler.GetType().IsDefined(typeof(ParallelExecutionAttribute), false))
                    {
                        Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), @event);
                    }
                    else
                    {
                        eventHandler.Handle(@event);
                    }
                }
            }
        }
        /// <summary>
        /// Publishes the event to all of its subscriptions.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to be published.</typeparam>
        /// <param name="event">The event to be published.</param>
        /// <param name="callback">The callback method to be executed after the event has been published and processed.</param>
        /// <param name="timeout">When the event handler is executing in parallel, represents the timeout value
        /// for the handler to complete.</param>
        public void Publish<TEvent>(TEvent @event, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null)
            where TEvent : class, IEvent
        {
            if (@event == null)
                throw new ArgumentNullException("evnt");
            var eventType = @event.GetType();
            if (eventHandlers.ContainsKey(eventType) &&
                eventHandlers[eventType] != null &&
                eventHandlers[eventType].Count > 0)
            {
                var handlers = eventHandlers[eventType];
                List<Task> tasks = new List<Task>();
                try
                {
                    foreach (var handler in handlers)
                    {
                        var eventHandler = handler as IEventHandler<TEvent>;
                        if (eventHandler.GetType().IsDefined(typeof(ParallelExecutionAttribute), false))
                        {
                            tasks.Add(Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), @event));
                        }
                        else
                        {
                            eventHandler.Handle(@event);
                        }
                    }
                    if (tasks.Count > 0)
                    {
                        if (timeout == null)
                            Task.WaitAll(tasks.ToArray());
                        else
                            Task.WaitAll(tasks.ToArray(), timeout.Value);
                    }
                    callback(@event, true, null);
                }
                catch (Exception ex)
                {
                    callback(@event, false, ex);
                }
            }
            else
                callback(@event, false, null);
        }
        #endregion
    }
}
