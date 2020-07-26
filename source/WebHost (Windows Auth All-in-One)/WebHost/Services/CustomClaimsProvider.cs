using IdentityModel;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebHost.Services
{
    public class CustomClaimsProvider : DefaultClaimsProvider
    {
        public CustomClaimsProvider(IUserService users) : base(users)
        {
        }

        public async override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client, IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            var claims = await base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            return claims.Concat(GetCustomClaims(subject)).Distinct(new ClaimComparer());
        }

        private IEnumerable<Claim> GetCustomClaims(ClaimsPrincipal subject)
        {
            var claims = new List<Claim>();

            // Adding claims, based on assigned roles
            var currentUserId = subject.FindFirst("sub").Value;
            var currentUserNAme = subject.Identity.Name;

            claims.AddRange(new Claim[]
            {
                new Claim(JwtClaimTypes.Role, "TASFD"),
                new Claim(JwtClaimTypes.Role, "LQWER"),
                new Claim(JwtClaimTypes.Role, "JVMQI"),
                new Claim(JwtClaimTypes.Role, "ASDF"),
            });

            return claims;
        }
    }
}
