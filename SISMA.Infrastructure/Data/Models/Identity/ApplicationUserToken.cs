using Microsoft.AspNetCore.Identity;

namespace SISMA.Infrastructure.Data.Models.Identity
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}