using Microsoft.Owin;
using Owin;
using Configuration;
using IdentityServer.WindowsAuthentication.Configuration;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.Owin.Security.WsFederation;
using Serilog;
using IdentityServer3.Host.Config;
using System.Threading.Tasks;
using IdentityServer.WindowsAuthentication.Services;
using System.Security.Claims;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            app.Map("/windows", win =>
            {
                win.UseWindowsAuthenticationService(new WindowsAuthenticationOptions
                 {
                     IdpRealm = "urn:idsrv3",
                     IdpReplyUrl = "https://localhost:44333/was",
                     SigningCertificate = Certificate.Load(),
                     EnableOAuth2Endpoint = true,
                     CustomClaimsProvider = new AdditionalWindowsClaimsProvider()
                 });
            });

            app.Map("/identity", identityServer =>
            {
                var identityServerFactory = new IdentityServerServiceFactory().Configure();
                identityServer.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "IdentityServer",
                    SigningCertificate = Certificate.Load(),
                    Factory = identityServerFactory,
                    
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnableLocalLogin = false
                    }
                });
            });
           
        }
    }

    public class AdditionalWindowsClaimsProvider : ICustomClaimsProvider
    {
        public async Task TransformAsync(CustomClaimsProviderContext context)
        {
            var email = await GetEmailFromActiveDirectoryAsync(context.OutgoingSubject);
            context.OutgoingSubject.AddClaim(new Claim(ClaimTypes.Email, email));
        }

        /// <summary>
        /// In a real implemention you look for an email in a mapping table or most likely in active directory.
        /// 
        /// This hard-coded email however will trigger the idenity server to ask for additional information since
        /// the user is not known yet in the user store and he used windows authentication to authenticate.
        /// </summary>
        /// <param name="outgoingSubject"></param>
        /// <returns></returns>
        private Task<string> GetEmailFromActiveDirectoryAsync(ClaimsIdentity outgoingSubject)
        {
            return Task.FromResult("foo@bar.com");
        }
    }
}