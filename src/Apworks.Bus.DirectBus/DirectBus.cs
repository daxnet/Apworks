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

using System.Collections.Generic;

namespace Apworks.Bus.DirectBus
{
    /// <summary>
    /// Represents the message bus that will dispatch the messages immediately once
    /// the bus is committed.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public abstract class DirectBus<TMessage> : DisposableObject, IBus<TMessage>
    {
        #region Private Fields
        private volatile bool committed = true;
        private readonly IMessageDispatcher dispatcher;
        private readonly Queue<TMessage> messageQueue = new Queue<TMessage>();
        private readonly object queueLock = new object();
        private TMessage[] backupMessageArray;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>DirectBus&lt;TMessage&gt;</c> class.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Apworks.Bus.IMessageDispatcher"/> which
        /// dispatches messages in the bus.</param>
        public DirectBus(IMessageDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.Committed)
                {
                    try
                    {
                        this.Commit();
                    }
                    catch
                    {
                        this.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region IBus<TMessage> Members
        /// <summary>
        /// Publishes the specified message to the bus.
        /// </summary>
        /// <param name="message">The message to be published.</param>
        public void Publish(TMessage message)
        {
            lock (queueLock)
            {
                messageQueue.Enqueue(message);
                committed = false;
            }
        }
        /// <summary>
        /// Publishes a collection of messages to the bus.
        /// </summary>
        /// <param name="messages">The messages to be published.</param>
        public void Publish(IEnumerable<TMessage> messages)
        {
            lock (queueLock)
            {
                foreach (var message in messages)
                {
                    messageQueue.Enqueue(message);
                }
                committed = false;
            }
        }
        /// <summary>
        /// Clears the published messages waiting for commit.
        /// </summary>
        public void Clear()
        {
            lock (queueLock)
            {
                this.messageQueue.Clear();
            }
        }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public bool DTCompatible
        {
            get { return false; }
        }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work was successfully committed.
        /// </summary>
        public bool Committed
        {
            get { return this.committed; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            lock (queueLock)
            {
                backupMessageArray = new TMessage[messageQueue.Count];
                messageQueue.CopyTo(backupMessageArray, 0);
                while (messageQueue.Count > 0)
                {
                    dispatcher.DispatchMessage(messageQueue.Dequeue());
                }
                committed = true;
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public void Rollback()
        {
            lock (queueLock)
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
