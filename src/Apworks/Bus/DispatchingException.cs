using System;
using System.Runtime.InteropServices;

namespace Apworks.Bus
{
    /// <summary>
    /// Represents the errors occur when dispatching the messages.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(_Exception))]
    public class DispatchingException : InfrastructureException
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>DispatcherException</c> class.
        /// </summary>
        public DispatchingException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <c>DispatcherException</c> class with the specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DispatchingException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <c>DispatcherException</c> class with the specified
        /// error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public DispatchingException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the <c>DispatcherException</c> class with the specified
        /// string formatter and the arguments that are used for formatting the message which
        /// describes the error.
        /// </summary>
        /// <param name="format">The string formatter which is used for formatting the error message.</param>
        /// <param name="args">The arguments that are used by the formatter to build the error message.</param>
        public DispatchingException(string format, params object[] args) : base(string.Format(format, args)) { }
        #endregion
    }
}
