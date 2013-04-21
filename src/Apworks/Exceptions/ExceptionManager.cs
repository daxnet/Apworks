using System;
using System.Collections.Generic;
using System.Linq;
using Apworks.Application;
using Apworks.Config;

namespace Apworks.Exceptions
{
    /// <summary>
    /// Represents the exception manager which handles and processes
    /// the exceptions.
    /// </summary>
    public sealed class ExceptionManager
    {
        #region Private Fields
        private static readonly ExceptionManager instance = new ExceptionManager();
        private readonly Dictionary<Type, ExceptionConfigItem> handlersOrig = new Dictionary<Type, ExceptionConfigItem>();
        private readonly Dictionary<Type, List<IExceptionHandler>> handlersResponsibilityChain = new Dictionary<Type, List<IExceptionHandler>>();
        #endregion

        #region Ctor
        static ExceptionManager() { }
        private ExceptionManager()
        {
            try
            {
                ApworksConfigSection config = AppRuntime.Instance.CurrentApplication.ConfigSource.Config;
                if (config.Exceptions != null &&
                    config.Exceptions.Count > 0)
                {
                    ExceptionElementCollection exceptionElementCollection = config.Exceptions;
                    foreach (ExceptionElement exceptionElement in exceptionElementCollection)
                    {
                        Type exceptionType = Type.GetType(exceptionElement.Type);
                        if (exceptionType == null)
                            continue;
                        
                        if (exceptionType.IsAbstract ||
                            !typeof(Exception).IsAssignableFrom(exceptionType))
                            continue;

                        ExceptionHandlingBehavior handlingBehavior = exceptionElement.Behavior;
                        if (exceptionElement.Handlers != null &&
                            exceptionElement.Handlers.Count > 0)
                        {
                            foreach (ExceptionHandlerElement exceptionHandlerElement in exceptionElement.Handlers)
                            {
                                Type handlerType = Type.GetType(exceptionHandlerElement.Type);
                                if (handlerType != null)
                                {
                                    if (handlerType.IsAbstract ||
                                        !handlerType.GetInterfaces().Any(p => p.Equals(typeof(IExceptionHandler))))
                                        continue;

                                    try
                                    {
                                        IExceptionHandler exceptionHandler = (IExceptionHandler)Activator.CreateInstance(handlerType);
                                        this.RegisterHandlerOrig(exceptionType, handlingBehavior, exceptionHandler);
                                    }
                                    catch
                                    {
                                        continue;
                                    } // try
                                } // if
                            } // foreach - exception handler
                        }
                        else
                        {
                            handlersOrig.Add(exceptionType, new ExceptionConfigItem { Behavior = handlingBehavior, Handlers = new List<IExceptionHandler>() });
                        }
                    } // foreach - exception
                    BuildResponsibilityChain();
                } // if
            }
            catch { }
        }
        #endregion

        #region Private Methods
        private void RegisterHandlerOrig(Type exceptionType, ExceptionHandlingBehavior behavior, IExceptionHandler handler)
        {
            if (handlersOrig.ContainsKey(exceptionType))
            {
                var exceptionConfigItem = handlersOrig[exceptionType];
                var list = exceptionConfigItem.Handlers;
                if (!list.Contains(handler, new ExceptionHandlerComparer()))
                {
                    list.Add(handler);
                }
            }
            else
            {
                ExceptionConfigItem configItem = new ExceptionConfigItem();
                configItem.Behavior = behavior;
                configItem.Handlers.Add(handler);
                handlersOrig[exceptionType] = configItem;
            }
        }

        private List<IExceptionHandler> DumpBaseHandlers(Type thisType)
        {
            List<IExceptionHandler> handlers = new List<IExceptionHandler>();
            Type baseType = thisType.BaseType;
            while (baseType != typeof(object))
            {
                if (handlersOrig.ContainsKey(baseType))
                {
                    var item = handlersOrig[baseType];
                    item.Handlers.ForEach(p => handlers.Add(p));
                    //break;
                }
                baseType = baseType.BaseType;
            }
            return handlers;
        }

        private void BuildResponsibilityChain()
        {
            foreach (var kvp in handlersOrig)
            {
                List<IExceptionHandler> handlers = new List<IExceptionHandler>();
                kvp.Value.Handlers.ForEach(p => handlers.Add(p));
                switch (kvp.Value.Behavior)
                {
                    case ExceptionHandlingBehavior.Direct:
                        break;
                    case ExceptionHandlingBehavior.Forward:
                        List<IExceptionHandler> handlersFromBase = DumpBaseHandlers(kvp.Key);
                        handlersFromBase.ForEach(p => handlers.Add(p));
                        break;
                    default:
                        break;
                }
                handlersResponsibilityChain.Add(kvp.Key, handlers);
            }
        }

        private bool HandleExceptionInternal(Exception ex)
        {
            Type exceptionType = ex.GetType();
            Type curType = exceptionType;
            while (curType != null && curType.IsClass && typeof(Exception).IsAssignableFrom(curType))
            {
                if (handlersResponsibilityChain.ContainsKey(curType))
                {
                    var handlers = handlersResponsibilityChain[curType];
                    if (handlers != null && handlers.Count > 0)
                    {
                        bool ret = false;
                        handlers.ForEach(p => ret |= p.HandleException(ex));
                        return ret; // if true, the exception was handled by at least one handler. otherwise false.
                    }
                    else
                        return false; // the exception was not handled.
                }
                curType = curType.BaseType;
            }
            return false; // no handler would handle the exception.
        }

        private IEnumerable<IExceptionHandler> GetExceptionHandlersInternal(Type exceptionType)
        {
            Type curType = exceptionType;
            while (curType != null && curType.IsClass && typeof(Exception).IsAssignableFrom(curType))
            {
                if (handlersResponsibilityChain.ContainsKey(curType))
                    return handlersResponsibilityChain[curType];
                curType = curType.BaseType;
            }
            return new List<IExceptionHandler>();
        }
        
        private IEnumerable<IExceptionHandler> GetExceptionHandlersInternal<TException>()
            where TException : Exception
        {
            return GetExceptionHandlers(typeof(TException));
        }
        
        private IEnumerable<Type> GetRegisteredExceptionTypesInternal()
        {
            return handlersResponsibilityChain.Keys;
        }
        #endregion

        #region Private Properties
        private static ExceptionManager InstanceInternal
        {
            get { return instance; }
        }
        private int RegisteredExceptionCountInternal
        {
            get { return handlersResponsibilityChain.Count; }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a <see cref="System.Int32"/> value which represents the number of exceptions
        /// registered in the exception manager.
        /// </summary>
        public static int RegisteredExceptionCount
        {
            get { return ExceptionManager.InstanceInternal.RegisteredExceptionCountInternal; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a list of exception handlers for a specific exception type.
        /// </summary>
        /// <param name="exceptionType">The type of the exception.</param>
        /// <returns>A list of exception handlers.</returns>
        public static IEnumerable<IExceptionHandler> GetExceptionHandlers(Type exceptionType)
        {
            return ExceptionManager.InstanceInternal.GetExceptionHandlersInternal(exceptionType);
        }
        /// <summary>
        /// Gets a list of exception handlers for a specific exception type.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns>A list of exception handlers.</returns>
        public static IEnumerable<IExceptionHandler> GetExceptionHandlers<TException>()
            where TException : Exception
        {
            return ExceptionManager.InstanceInternal.GetExceptionHandlersInternal<TException>();
        }
        /// <summary>
        /// Gets all the exception types registered in the Apworks configuration section.
        /// </summary>
        /// <returns>A list of exception types registered.</returns>
        public static IEnumerable<Type> GetRegisteredExceptionTypes()
        {
            return ExceptionManager.InstanceInternal.GetRegisteredExceptionTypesInternal();
        }
        /// <summary>
        /// Handles a specific exception.
        /// </summary>
        /// <param name="ex">The exception to be handled.</param>
        /// <returns>True if the exception can be handled successfully, otherwise, false.</returns>
        public static bool HandleException(Exception ex)
        {
            return ExceptionManager.InstanceInternal.HandleExceptionInternal(ex);
        }
        /// <summary>
        /// Handles a specific exception.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to be handled.</typeparam>
        /// <param name="ex">The exception to be handled.</param>
        /// <returns>True if the exception can be handled successfully, otherwise, false.</returns>
        public static bool HandleException<TException>(TException ex)
            where TException : Exception
        {
            return ExceptionManager.InstanceInternal.HandleExceptionInternal((Exception)ex);
        }
        #endregion

        #region Internal Classes
        class ExceptionHandlerComparer : IEqualityComparer<IExceptionHandler>
        {
            public bool Equals(IExceptionHandler x, IExceptionHandler y)
            {
                return x.GetType().AssemblyQualifiedName.Equals(y.GetType().AssemblyQualifiedName);
            }

            public int GetHashCode(IExceptionHandler obj)
            {
                return obj.GetHashCode();
            }
        }

        class ExceptionConfigItem
        {
            public ExceptionHandlingBehavior Behavior { get; set; }
            public List<IExceptionHandler> Handlers { get; set; }

            public ExceptionConfigItem()
            {
                Behavior = ExceptionHandlingBehavior.Direct;
                Handlers = new List<IExceptionHandler>();
            }
        }
        #endregion
    }
}
