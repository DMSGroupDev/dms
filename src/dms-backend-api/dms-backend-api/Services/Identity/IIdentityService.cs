
using dms_backend_api.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.Services.Identity
{
    public partial interface IIdentityService
    {
        RoleManager<ApplicationRole> GetRoleManager();
        UserManager<ApplicationUser> GetUserManager();
        SignInManager<ApplicationUser> GetSignInManager();
    }
}