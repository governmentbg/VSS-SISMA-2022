using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SISMA.Infrastructure.Data.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

        public string Label { get; set; }
    }
}
