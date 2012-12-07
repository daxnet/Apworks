using System;

namespace Apworks.Exceptions
{
    /// <summary>
    /// Represents the base class of exception handlers.
    /// </summary>
    /// <typeparam name="TException">The type of the exception being handled.</typeparam>
    public abstract class ExceptionHandler<TException> : IExceptionHandler
        where TException : Exception
    {
        #region Protected Methods
        /// <summary>
        /// Performs the exception handling internally.
        /// </summary>
        /// <param name="ex">The exception to be handled.</param>
        /// <returns>True if the exception was handled successfully, otherwise, false.</returns>
        protected abstract bool DoHandle(TException ex);
        #endregion

        #region Public Methods
        /// <summary>
        /// Handles a specific exception.
        /// </summary>
        /// <param name="ex">The exception to be handled.</param>
        /// <returns>True if the exceptioin was successfully handled, otherwise, false.</returns>
        public virtual bool HandleException(Exception ex)
        {
            return DoHandle(ex as TException);
        }
        #endregion
    }
}
