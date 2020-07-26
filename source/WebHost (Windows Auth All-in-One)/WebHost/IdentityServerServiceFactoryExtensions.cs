using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using System;
using WebHost.Services;

namespace WebHost
{
    public static class IdentityServerServiceFactoryExtensions
    {
        public static IdentityServerServiceFactory Configure(this IdentityServerServiceFactory factory)
        {
            factory.CustomGrantValidators.Add(new Registration<ICustomGrantValidator>(typeof(WindowsGrantValidator)));
            factory.UserService = new Registration<IUserService>(new ExternalRegistrationUserService());
            factory.ClaimsProvider = new Registration<IClaimsProvider>(typeof(CustomClaimsProvider));
            factory.UseInMemoryClients(Config.Clients);
            factory.UseInMemoryScopes(Config.Scopes);

            return factory;
        }
    }
}
