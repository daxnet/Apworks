using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IConfigurator<TContainer>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        TContainer Configure();
    }

    public abstract class Configurator<TContainer> : IConfigurator<TContainer>
    {
        private readonly IConfigurator<TContainer> context;

        public Configurator(IConfigurator<TContainer> context)
        {
            this.context = context;
        }

        public IConfigurator<TContainer> Context
        {
            get { return this.context; }
        }

        protected abstract TContainer DoConfigure(TContainer container);

        #region IConfigurator<TContainer> Members

        public TContainer Configure()
        {
            var container = this.context.Configure();
            return DoConfigure(container);
        }

        #endregion
    }
}
