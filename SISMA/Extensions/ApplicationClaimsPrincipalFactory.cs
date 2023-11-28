using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SISMA.Extensions
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly IRepository repo;
        public ApplicationClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options,
            IRepository _repo) : base(userManager, roleManager, options)
        {
            repo = _repo;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(CustomClaimType.FullName, user.FullName));
            return principal;
        }



    }
}
