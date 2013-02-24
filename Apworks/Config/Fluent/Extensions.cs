using Apworks.Application;
using Apworks.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apworks.Config.Fluent
{
    public static class Extensions
    {
        #region IApworksConfigurator Extenders
        public static IApplicationConfigurator WithApplication<TApplication>(this IApworksConfigurator configurator)
            where TApplication : IApp
        {
            return new ApplicationConfigurator(configurator, typeof(TApplication));
        }

        public static IApplicationConfigurator WithDefaultApplication(this IApworksConfigurator configurator)
        {
            return WithApplication<App>(configurator);
        }

        public static ISequenceGeneratorConfigurator WithDefaultApplicationAndGenerators(this IApworksConfigurator configurator)
        {
            return WithDefaultSequenceGenerator(WithDefaultIdentityGenerator(WithDefaultApplication(configurator)));
        }
        #endregion

        #region IAppConfigurator Extenders
        public static IIdentityGeneratorConfigurator WithIdentityGenerator<TIdentityGenerator>(this IApplicationConfigurator configurator)
            where TIdentityGenerator : IIdentityGenerator
        {
            return new IdentityGeneratorConfigurator(configurator, typeof(TIdentityGenerator));
        }

        public static IIdentityGeneratorConfigurator WithDefaultIdentityGenerator(this IApplicationConfigurator configurator)
        {
            return WithIdentityGenerator<IdentityGenerator>(configurator);
        }
        #endregion

        #region IIdentityGeneratorConfigurator Extenders
        public static ISequenceGeneratorConfigurator WithSequenceGenerator<TSequenceGenerator>(this IIdentityGeneratorConfigurator configurator)
            where TSequenceGenerator : ISequenceGenerator
        {
            return new SequenceGeneratorConfigurator(configurator, typeof(TSequenceGenerator));
        }

        public static ISequenceGeneratorConfigurator WithDefaultSequenceGenerator(this IIdentityGeneratorConfigurator configurator)
        {
            return WithSequenceGenerator<SequenceGenerator>(configurator);
        }
        #endregion

        #region ISequenceGeneratorConfigurator Extenders
        public static IObjectContainerConfigurator WithObjectContainer<TObjectContainer>(this ISequenceGeneratorConfigurator configurator)
            where TObjectContainer : IObjectContainer
        {
            return new ObjectContainerConfigurator(configurator, typeof(TObjectContainer));
        }
        #endregion

        #region IObjectContainerConfigurator
        public static IApp Create(IObjectContainerConfigurator configurator)
        {
            var configSource = configurator.Configure();
            var appInstance = AppRuntime.Create(configSource);
            return appInstance;
        }
        #endregion

        #region AppRuntime Instance Extenders
        public static IApworksConfigurator ConfigureApworks(this AppRuntime appRuntime)
        {
            return new ApworksConfigurator();
        }
        #endregion
    }
}
