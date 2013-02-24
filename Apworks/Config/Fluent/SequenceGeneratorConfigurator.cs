using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface ISequenceGeneratorConfigurator : IConfigSourceConfigurator
    { }

    public class SequenceGeneratorConfigurator : TypeSpecifiedConfigSourceConfigurator, ISequenceGeneratorConfigurator
    {
        public SequenceGeneratorConfigurator(IConfigSourceConfigurator context, Type sequenceGeneratorType)
            : base(context, sequenceGeneratorType)
        { }

        protected override IConfigSource DoConfigure(IConfigSource container)
        {
            container.Config.Generators.SequenceGenerator.Provider = Type.AssemblyQualifiedName;
            return container;
        }
    }
}
