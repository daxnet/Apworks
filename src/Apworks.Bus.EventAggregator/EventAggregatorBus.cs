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

using Apworks.Events;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apworks.Bus.EventAggregator
{
    /// <summary>
    /// Represents the bus implemented by using the event aggregator.
    /// </summary>
    public abstract class EventAggregatorBus : DisposableObject, IBus
    {
        #region Private Fields
        private readonly Queue<object> messageQueue = new Queue<object>();
        private readonly IEventAggregator eventAggregator;
        private readonly MethodInfo publishMethod;
        private readonly object sync = new object();
        private bool committed = true;
        private object[] backupMessageArray;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>EventAggregatorBus</c> class.
        /// </summary>
        /// <param name="eventAggregator">The <see cref="IEventAggregator"/> instance.</param>
        public EventAggregatorBus(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            publishMethod = (from m in this.eventAggregator.GetType().GetMethods()
                             let parameters = m.GetParameters()
                             let methodName = m.Name
                             where methodName == "Publish" &&
                             parameters != null &&
                             parameters.Length == 1
                             select m).First();
        }
        #endregion

        #region Private Methods
        private void PublishInternal<TMessage>(TMessage message)
        {
            messageQueue.Enqueue(message);
            committed = false;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing) { }
        #endregion

        #region IBus Members
        /// <summary>
        /// Publishes the specified message to the bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message to be published.</param>
        public void Publish<TMessage>(TMessage message)
        {
            lock (sync)
            {
                PublishInternal<TMessage>(message);
            }
        }
        /// <summary>
        /// Publishes a collection of messages to the bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="messages">The messages to be published.</param>
        public void Publish<TMessage>(IEnumerable<TMessage> messages)
        {
            lock (sync)
            {
                foreach (var message in messages)
                    PublishInternal<TMessage>(message);
            }
        }
        /// <summary>
        /// Clears the published messages waiting for commit.
        /// </summary>
        public void Clear()
        {
            lock (sync)
            {
                messageQueue.Clear();
                committed = true;
            }
        }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public bool DistributedTransactionSupported
        {
            get { return false; }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public bool Committed
        {
            get { return committed; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            lock (sync)
            {
                backupMessageArray = new object[messageQueue.Count];
                messageQueue.CopyTo(backupMessageArray, 0);
                while (messageQueue.Count > 0)
                {
                    var @event = messageQueue.Dequeue();
                    var @eventType = @event.GetType();
                    var method = publishMethod.MakeGenericMethod(@eventType);
                    method.Invoke(this.eventAggregator, new object[] { @event });
                }
                committed = true;
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public void Rollback()
        {
            lock (sync)
            {
                if (backupMessageArray != null && backupMessageArray.Length > 0)
                {
                    messageQueue.Clear();
                    foreach (var msg in backupMessageArray)
                    {
                        messageQueue.Enqueue(msg);
                    }
                }
                committed = false;
            }
        }

        #endregion
    }
}
