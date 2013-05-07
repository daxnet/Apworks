using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection;

namespace Apworks.Bus.MSMQ
{
    /// <summary>
    /// Represents the base class for buses that are using Microsoft Message Queuing (MSMQ) technologies.
    /// </summary>
    public abstract class MSMQBus : DisposableObject, IBus
    {
        #region Private Fields
        private volatile bool committed = true;
        private readonly bool useInternalTransaction;
        private readonly MSMQBusOptions options;
        private readonly MessageQueue messageQueue;
        private readonly object lockObj = new object();
        private readonly Queue<object> mockQueue = new Queue<object>();
        private readonly MethodInfo sendMessageGenericMethod;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MSMQBus</c> class.
        /// </summary>
        public MSMQBus()
        {
            sendMessageGenericMethod = (from m in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                        let methodName = m.Name
                                        let generic = m.IsGenericMethod
                                        where generic &&
                                        methodName == "SendMessageGeneric"
                                        select m).First();
        }
        /// <summary>
        /// Initializes a new instance of <c>MSMQBus&lt;TMessage&gt;</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        public MSMQBus(string path)
            : this()
        {
            this.options = new MSMQBusOptions(path);
            this.messageQueue = new MessageQueue(path,
                options.SharedModeDenyReceive,
                options.EnableCache, options.QueueAccessMode);
            this.messageQueue.Formatter = options.MessageFormatter;
            this.useInternalTransaction = options.UseInternalTransaction && messageQueue.Transactional;
        }
        /// <summary>
        /// Initializes a new instance of <c>MSMQBus&lt;TMessage&gt;</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        /// <param name="useInternalTransaction">A <see cref="System.Boolean"/> value which indicates
        /// whether the internal transaction should be used to manipulate the message queue.</param>
        public MSMQBus(string path, bool useInternalTransaction)
            : this()
        {
            this.options = new MSMQBusOptions(path, useInternalTransaction);
            this.messageQueue = new MessageQueue(path,
                options.SharedModeDenyReceive,
                options.EnableCache, options.QueueAccessMode);
            this.messageQueue.Formatter = options.MessageFormatter;
            this.useInternalTransaction = options.UseInternalTransaction && messageQueue.Transactional;
        }
        /// <summary>
        /// Initializes a new instance of <c>MSMQBus&lt;TMessage&gt;</c> class.
        /// </summary>
        /// <param name="options">The instance of <see cref="Apworks.Bus.MSMQ.MSMQBusOptions"/> class
        /// which contains the option information to initialize the message queue.</param>
        public MSMQBus(MSMQBusOptions options)
            : this()
        {
            this.options = options;
            this.messageQueue = new MessageQueue(options.Path,
                options.SharedModeDenyReceive,
                options.EnableCache, options.QueueAccessMode);
            this.messageQueue.Formatter = options.MessageFormatter;
            this.useInternalTransaction = options.UseInternalTransaction && messageQueue.Transactional;
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets the location of the queue.
        /// </summary>
        protected string Path { get { return options.Path; } }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates whether 
        /// to create and use a connection cache.
        /// </summary>
        protected bool EnableCache { get { return options.EnableCache; } }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates whether
        /// to grant exclusive read access to the first application that accesses
        /// the queue.
        /// </summary>
        protected bool SharedModeDenyReceive { get { return options.SharedModeDenyReceive; } }
        /// <summary>
        /// Gets a <see cref="System.Messaging.QueueAccessMode"/> value which specifies the 
        /// access mode for the queue at creation time.
        /// </summary>
        protected QueueAccessMode QueueAccessMode { get { return options.QueueAccessMode; } }
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates whether an internal transaction
        /// should be used when manipulating the queue.
        /// </summary>
        protected bool UseInternalTransaction { get { return this.useInternalTransaction; } }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Sends the specified message to the queue.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// <param name="transaction">The <see cref="MessageQueueTransaction"/> instance.</param>
        protected void SendMessageGeneric<TMessage>(TMessage message, MessageQueueTransaction transaction = null)
        {
            Message msmqMessage = new Message(message);
            if (useInternalTransaction)
            {
                if (transaction == null)
                    throw new ArgumentNullException("transaction");

                messageQueue.Send(msmqMessage, transaction);
            }
            else
            {
                messageQueue.Send(msmqMessage, MessageQueueTransactionType.Automatic);
            }
        }
        /// <summary>
        /// Sends the specified message to the queue.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="transaction">The <see cref="MessageQueueTransaction"/> instance.</param>
        protected void SendMessage(object message, MessageQueueTransaction transaction = null)
        {
            var eventType = message.GetType();
            sendMessageGenericMethod.MakeGenericMethod(eventType).Invoke(this, new object[] { message, transaction });
        }
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
                if (messageQueue != null)
                {
                    messageQueue.Close();
                    messageQueue.Dispose();
                }
            }
        }
        #endregion

        #region IBus<TMessage> Members
        /// <summary>
        /// Publishes the specified message to the bus.
        /// </summary>
        /// <param name="message">The message to be published.</param>
        public void Publish<TMessage>(TMessage message)
        {
            lock (lockObj)
            {
                mockQueue.Enqueue(message);
                committed = false;
            }
        }
        /// <summary>
        /// Publishes a collection of messages to the bus.
        /// </summary>
        /// <param name="messages">The messages to be published.</param>
        public void Publish<TMessage>(IEnumerable<TMessage> messages)
        {
            lock (lockObj)
            {
                messages.ToList().ForEach(p => { mockQueue.Enqueue(p); committed = false; });
            }
        }
        /// <summary>
        /// Clears the published messages waiting for commit.
        /// </summary>
        public void Clear()
        {
            lock (lockObj)
            {
                this.mockQueue.Clear();
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
            get { return true; }
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
            lock (lockObj)
            {
                if (this.useInternalTransaction)
                {
                    //backupMessageArray = new TMessage[mockQueue.Count];
                    //mockQueue.CopyTo(backupMessageArray, 0);
                    using (MessageQueueTransaction transaction = new MessageQueueTransaction())
                    {
                        try
                        {
                            transaction.Begin();
                            while (mockQueue.Count > 0)
                            {
                                object msg = mockQueue.Dequeue();
                                SendMessage(msg, transaction);
                            }
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Abort();
                            throw;
                        }
                    }
                }
                else
                {
                    while (mockQueue.Count > 0)
                    {
                        object msg = mockQueue.Dequeue();
                        SendMessage(msg);
                    }
                }
                committed = true;
            }
        }
        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public void Rollback()
        {
            committed = false;
        }
        #endregion
    }
}
