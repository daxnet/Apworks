using System;
using System.Runtime.InteropServices;

namespace Apworks
{
    /// <summary>
    /// Represents errors that occur in the domain layer of Apworks framework.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(_Exception))]
    public class DomainException : ApworksException
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>DomainException</c> class.
        /// </summary>
        public DomainException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <c>DomainException</c> class with the specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DomainException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <c>DomainException</c> class with the specified
        /// error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the <c>DomainException</c> class with the specified
        /// string formatter and the arguments that are used for formatting the message which
        /// describes the error.
        /// </summary>
        /// <param name="format">The string formatter which is used for formatting the error message.</param>
        /// <param name="args">The arguments that are used by the formatter to build the error message.</param>
        public DomainException(string format, params object[] args) : base(string.Format(format, args)) { }
        #endregion
    }
}
