using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public interface IApworksConfigurator : IConfigSourceConfigurator { }

    public class ApworksConfigurator : IApworksConfigurator
    {
        private readonly IConfigSource configSource = new RegularConfigSource();

        public ApworksConfigurator() { }

        #region IConfigurator<IConfigSource> Members

        public IConfigSource Configure()
        {
            return configSource;
        }

        #endregion
    }

}
