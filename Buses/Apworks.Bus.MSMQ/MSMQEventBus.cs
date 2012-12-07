using Apworks.Events;

namespace Apworks.Bus.MSMQ
{
    /// <summary>
    /// Represents the event bus which uses the Microsoft Message Queuing technology.
    /// </summary>
    public class MSMQEventBus : MSMQBus<IEvent>, IEventBus
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MSMQEventBus</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        public MSMQEventBus(string path) : base(path) { }
        /// <summary>
        /// Initializes a new instance of <c>MSMQEventBus</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        /// <param name="useInternalTransaction">A <see cref="System.Boolean"/> value which indicates
        /// whether the internal transaction should be used to manipulate the message queue.</param>
        public MSMQEventBus(string path, bool useInternalTransaction) : base(path, useInternalTransaction) { }
        /// <summary>
        /// Initializes a new instance of <c>MSMQEventBus</c> class.
        /// </summary>
        /// <param name="options">The instance of <see cref="Apworks.Bus.MSMQ.MSMQBusOptions"/> class
        /// which contains the option information to initialize the message queue.</param>
        public MSMQEventBus(MSMQBusOptions options) : base(options) { }
        #endregion
    }
}
