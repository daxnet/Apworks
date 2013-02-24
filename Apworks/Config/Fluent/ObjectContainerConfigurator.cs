using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IObjectContainerConfigurator : IConfigSourceConfigurator
    {
    }

    public class ObjectContainerConfigurator : TypeSpecifiedConfigSourceConfigurator, IObjectContainerConfigurator
    {
        public ObjectContainerConfigurator(IConfigSourceConfigurator context, Type objectContainerType)
            : base(context, objectContainerType)
        { }

        protected override IConfigSource DoConfigure(IConfigSource container)
        {
            container.Config.ObjectContainer.Provider = this.Type.AssemblyQualifiedName;
            return container;
        }
    }
}
