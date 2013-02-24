using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IApplicationConfigurator : IConfigSourceConfigurator
    { }

    public class ApplicationConfigurator : TypeSpecifiedConfigSourceConfigurator, IApplicationConfigurator
    {
        public ApplicationConfigurator(IConfigSourceConfigurator context, Type appType)
            : base(context, appType)
        { }

        protected override IConfigSource DoConfigure(IConfigSource container)
        {
            container.Config.Application.Provider = Type.AssemblyQualifiedName;
            return container;
        }
    }
}
