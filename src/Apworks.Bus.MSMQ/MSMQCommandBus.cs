using Apworks.Commands;

namespace Apworks.Bus.MSMQ
{
    /// <summary>
    /// Represents the command bus which uses Microsoft Message Queuing technology.
    /// </summary>
    public class MSMQCommandBus : MSMQBus<ICommand>, ICommandBus
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>MSMQCommandBus</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        public MSMQCommandBus(string path) : base(path) { }
        /// <summary>
        /// Initializes a new instance of <c>MSMQCommandBus</c> class.
        /// </summary>
        /// <param name="path">The location of the queue.</param>
        /// <param name="useInternalTransaction">A <see cref="System.Boolean"/> value which indicates
        /// whether the internal transaction should be used to manipulate the message queue.</param>
        public MSMQCommandBus(string path, bool useInternalTransaction) : base(path, useInternalTransaction) { }
        /// <summary>
        /// Initializes a new instance of <c>MSMQCommandBus</c> class.
        /// </summary>
        /// <param name="options">The instance of <see cref="Apworks.Bus.MSMQ.MSMQBusOptions"/> class
        /// which contains the option information to initialize the message queue.</param>
        public MSMQCommandBus(MSMQBusOptions options) : base(options) { }
        #endregion
    }
}
