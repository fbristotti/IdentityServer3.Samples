﻿using Owin;
using SelfHost.Config;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Host.Config;

namespace SelfHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var factory = InMemoryFactory.Create(
                users:   Users.Get(), 
                clients: Clients.Get(), 
                scopes:  Scopes.Get());

            var options = new IdentityServerOptions
            {
                IssuerUri = "https://idsrv3.com",
                SiteName = "Thinktecture IdentityServer v3 - preview 1 (SelfHost)",
                PublicHostName = "http://localhost:3333",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
            };

            appBuilder.UseIdentityServer(options);
        }

        //private void ConfigurePlugins(IAppBuilder pluginApp, IdentityServerOptions options)
        //{
        //    var wsFedOptions = new WsFederationPluginOptions
        //    {
        //        Factory = new WsFederationServiceFactory
        //        {
        //            UserService = options.Factory.UserService,
        //            CoreSettings = options.Factory.CoreSettings,
        //            RelyingPartyService = Registration.RegisterFactory<IRelyingPartyService>(() => new InMemoryRelyingPartyService(RelyingParties.Get())),
        //            WsFederationSettings = Registration.RegisterFactory<WsFederationSettings>(() => new WsFedSettings())
        //        },
        //        DataProtector = options.DataProtector
        //    };

        //    options.ProtocolLogoutUrls.Add(wsFedOptions.LogoutUrl);
        //    pluginApp.UseWsFederationPlugin(wsFedOptions);
        //}
    }
}