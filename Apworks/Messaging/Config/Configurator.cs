using Apworks.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Messaging.Config
{
    public abstract class Configurator : IConfigurator
    {
        private readonly IObjectContainer objectContainer;

        public Configurator()
        {
            objectContainer = AppRuntime.Instance.CurrentApplication.ObjectContainer;
        }

        internal IObjectContainer ObjectContainer
        {
            get { return objectContainer; }
        }

        #region IConfigurator Members
        public abstract IConfigurator Configure();
        #endregion
    }
}
