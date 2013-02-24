using System;
using Apworks.Application;

namespace Apworks
{
    /// <summary>
    /// Represents the service locator which locates a service with the given type.
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator
    {
        #region Private Fields
        private readonly IObjectContainer objectContainer = AppRuntime.Instance.CurrentApplication.ObjectContainer;
        private static readonly ServiceLocator instance = new ServiceLocator();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>ServiceLocator</c> class.
        /// </summary>
        private ServiceLocator() { }
        #endregion

        #region Public Static Properties
        /// <summary>
        /// Gets the current instance of <c>ServiceLocator</c> class.
        /// </summary>
        public static ServiceLocator Instance
        {
            get { return instance; }
        }
        #endregion

        #region IServiceLocator Members

        public T GetService<T>() where T : class
        {
            return objectContainer.GetService<T>();
        }

        public T GetService<T>(object overridedArguments) where T : class
        {
            return objectContainer.GetService<T>(overridedArguments);
        }

        public object GetService(Type serviceType, object overridedArguments)
        {
            return objectContainer.GetService(serviceType, overridedArguments);
        }

        public Array ResolveAll(Type serviceType)
        {
            return objectContainer.ResolveAll(serviceType);
        }

        public T[] ResolveAll<T>() where T : class
        {
            return objectContainer.ResolveAll<T>();
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return objectContainer.GetService(serviceType);
        }

        #endregion
    }
}
