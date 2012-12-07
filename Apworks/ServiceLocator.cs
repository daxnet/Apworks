using System;
using Apworks.Application;

namespace Apworks
{
    /// <summary>
    /// Represents the service locator which locates a service with the given type.
    /// </summary>
    public sealed class ServiceLocator : ObjectContainer
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

        #region Protected Methods
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType.-or- null if there is no service object
        /// of type serviceType.</returns>
        protected override object DoGetService(Type serviceType)
        {
            return objectContainer.GetService(serviceType);
        }
        /// <summary>
        /// Gets the service object of the specified type, with overrided
        /// arguments provided.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        protected override object DoGetService(Type serviceType, object overridedArguments)
        {
            return objectContainer.GetService(serviceType, overridedArguments);
        }
        /// <summary>
        /// Resolves all the objects from the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the objects to be resolved.</param>
        /// <returns>A <see cref="System.Array"/> object which contains all the objects resolved.</returns>
        protected override Array DoResolveAll(Type serviceType)
        {
            return objectContainer.ResolveAll(serviceType);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the object container by using the application/web config file.
        /// </summary>
        /// <param name="configSectionName">The name of the ConfigurationSection in the application/web config file
        /// which is used for initializing the object container.</param>
        public override void InitializeFromConfigFile(string configSectionName)
        {
            objectContainer.InitializeFromConfigFile(configSectionName);
        }
        /// <summary>
        /// Gets the wrapped container instance.
        /// </summary>
        /// <typeparam name="T">The type of the wrapped container.</typeparam>
        /// <returns>The instance of the wrapped container.</returns>
        public override T GetWrappedContainer<T>()
        {
            return objectContainer.GetWrappedContainer<T>();
        }
        #endregion
    }
}
