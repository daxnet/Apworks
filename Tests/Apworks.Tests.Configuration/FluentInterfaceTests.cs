using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apworks.Application;
using Apworks.Config.Fluent;
using Apworks.Config;
using Apworks.Repositories;
using Apworks.Repositories.NHibernate;

namespace Apworks.Tests.Configuration
{
    [TestClass]
    public class FluentInterfaceTests
    {
        //[TestMethod]
        //public void Configuration_CreateAppWithDefaultSettingsTest()
        //{
        //    var application = AppRuntime.Instance.ConfigureApworks().WithDefaultSettings().UsingUnityContainer().Create();
        //    application.Initialize += (s, e) =>
        //        {
                    
        //        };
        //    application.Start();
        //    Assert.IsNotNull(application);
        //}

        [TestMethod]
        public void Configuration_AddCQRSMessageHandlerTest()
        {
            var application = AppRuntime.Instance
                .ConfigureApworks()
                .WithDefaultSettings()
                .AddMessageHandler(HandlerKind.Command, HandlerSourceType.Assembly, "1")
                .AddMessageHandler(HandlerKind.Event, HandlerSourceType.Type, "2")
                .UsingUnityContainer()
                .Create();
            application.Initialize += (s, e) =>
            {
                
            };
            application.Start();
            Assert.IsNotNull(application);
            Assert.AreEqual(2, application.ConfigSource.Config.Handlers.Count);
        }

    }
}
