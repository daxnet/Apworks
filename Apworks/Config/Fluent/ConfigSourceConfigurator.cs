using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IConfigSourceConfigurator : IConfigurator<IConfigSource> { }

    public abstract class ConfigSourceConfigurator : Configurator<IConfigSource>
    {
        public ConfigSourceConfigurator(IConfigSourceConfigurator context)
            : base(context)
        {
        }
    }

}
