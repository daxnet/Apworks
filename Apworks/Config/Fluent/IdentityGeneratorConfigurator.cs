using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IIdentityGeneratorConfigurator : IConfigSourceConfigurator
    { }

    public class IdentityGeneratorConfigurator : TypeSpecifiedConfigSourceConfigurator, IIdentityGeneratorConfigurator
    {
        public IdentityGeneratorConfigurator(IConfigSourceConfigurator context, Type identityGeneratorType)
            : base(context, identityGeneratorType)
        { }

        protected override IConfigSource DoConfigure(IConfigSource container)
        {
            container.Config.Generators.IdentityGenerator.Provider = Type.AssemblyQualifiedName;
            return container;
        }
    }
}
