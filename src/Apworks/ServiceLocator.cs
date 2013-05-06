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
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the service object.</typeparam>
        /// <returns>The instance of the service object.</returns>
        public T GetService<T>() where T : class
        {
            return objectContainer.GetService<T>();
        }
        /// <summary>
        /// Gets the service object of the specified type, with overrided
        /// arguments provided.
        /// </summary>
        /// <typeparam name="T">The type of the service object.</typeparam>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        public T GetService<T>(object overridedArguments) where T : class
        {
            return objectContainer.GetService<T>(overridedArguments);
        }
        /// <summary>
        /// Gets the service object of the specified type, with overrided
        /// arguments provided.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        public object GetService(Type serviceType, object overridedArguments)
        {
            return objectContainer.GetService(serviceType, overridedArguments);
        }
        /// <summary>
        /// Resolves all the objects from the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the objects to be resolved.</param>
        /// <returns>A <see cref="System.Array"/> object which contains all the objects resolved.</returns>
        public Array ResolveAll(Type serviceType)
        {
            return objectContainer.ResolveAll(serviceType);
        }
        /// <summary>
        /// Resolves all the objects from the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the objects to be resolved.</typeparam>
        /// <returns>A <see cref="System.Array"/> object which contains all the objects resolved.</returns>
        public T[] ResolveAll<T>() where T : class
        {
            return objectContainer.ResolveAll<T>();
        }
        /// <summary>
        /// Returns a <see cref="Boolean"/> value which indicates whether the given type
        /// has been registered to the service locator.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>True if the type has been registered, otherwise, false.</returns>
        public bool Registered<T>()
        {
            return objectContainer.Registered<T>();
        }
        /// <summary>
        /// Returns a <see cref="Boolean"/> value which indicates whether the given type
        /// has been registered to the service locator.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type has been registered, otherwise, false.</returns>
        public bool Registered(Type type)
        {
            return objectContainer.Registered(type);
        }
        #endregion

        #region IServiceProvider Members
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <returns>The instance of the service object.</returns>
        public object GetService(Type serviceType)
        {
            return objectContainer.GetService(serviceType);
        }

        #endregion
    }
}
