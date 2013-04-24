using System;
using Microsoft.Practices.Unity;
using System.ServiceModel;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace Apworks.ObjectContainers.Unity
{
    /// <summary>
    /// Represents the WCF Service Instance extension of the <see cref="InstanceContext"/> class.
    /// </summary>
    class ContainerExtension : IExtension<OperationContext>
    {
        #region Members
        /// <summary>
        /// Gets or sets the value stored.
        /// </summary>
        public object Value { get; set; }

        #endregion

        #region IExtension<OperationContext> Members
        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called
        /// when the extension is added to the <see cref="System.ServiceModel.IExtensibleObject&lt;T&gt;.Extensions"/>
        /// property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(OperationContext owner)
        {

        }
        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when
        /// an extension is removed from the <see cref="System.ServiceModel.IExtensibleObject&lt;T&gt;.Extensions"/>
        /// property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(OperationContext owner)
        {

        }

        #endregion
    }

    /// <summary>
    /// Represents the lifetime manager which controls the lifetime of the instances based
    /// on each WCF call.
    /// </summary>
    public class PerRequestLifetimeManager : LifetimeManager
    {
        #region Private Fields
        private readonly Guid key = Guid.NewGuid();
        #endregion

        /// <summary>
        /// Initializes a new instance of <c>WcfPerRequestLifetimeManager</c> class.
        /// </summary>
        public PerRequestLifetimeManager() : this(Guid.NewGuid()) { }

        PerRequestLifetimeManager(Guid key)
        {
            if (key == Guid.Empty)
                throw new ArgumentException("Key is empty.");

            this.key = key;
        }

        #region Public Methods
        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>The object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            object result = null;

            //Get object depending on  execution environment ( WCF without HttpContext,HttpContext or CallContext)

            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                ContainerExtension containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension != null)
                {
                    result = containerExtension.Value;
                }
            }
            else if (HttpContext.Current != null)
            {
                //HttpContext avaiable ( ASP.NET ..)
                if (HttpContext.Current.Items[key.ToString()] != null)
                    result = HttpContext.Current.Items[key.ToString()];
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                result = CallContext.GetData(key.ToString());
            }

            return result;
        }
        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                ContainerExtension containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension != null)
                    OperationContext.Current.Extensions.Remove(containerExtension);

            }
            else if (HttpContext.Current != null)
            {
                //HttpContext avaiable ( ASP.NET ..)
                if (HttpContext.Current.Items[key.ToString()] != null)
                    HttpContext.Current.Items[key.ToString()] = null;
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                CallContext.FreeNamedDataSlot(key.ToString());
            }
        }
        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                ContainerExtension containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension == null)
                {
                    containerExtension = new ContainerExtension()
                    {
                        Value = newValue
                    };

                    OperationContext.Current.Extensions.Add(containerExtension);
                }
            }
            else if (HttpContext.Current != null)
            {
                //HttpContext avaiable ( ASP.NET ..)
                if (HttpContext.Current.Items[key.ToString()] == null)
                    HttpContext.Current.Items[key.ToString()] = newValue;
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                CallContext.SetData(key.ToString(), newValue);
            }
        }
        #endregion
    }
}
