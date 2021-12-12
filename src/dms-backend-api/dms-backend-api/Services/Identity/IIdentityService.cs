
using dms_backend_api.Domain;
using dms_backend_api.Domain.Identity;
using dms_backend_api.Domain.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace dms_backend_api.Services.Identity
{
    public partial interface IIdentityService
    {
        RoleManager<ApplicationRole> GetRoleManager();
        UserManager<ApplicationUser> GetUserManager();
        SignInManager<ApplicationUser> GetSignInManager();

        Task<bool> ValidationRegisterDomainAsync(Domains validateRegisterDomain);
        Task<DbResult> RegisterDomainAsync(Domains registerDomains, Guid ownerId);
        Task<DbResult> AddUserToDomainAsync(Domains registerDomains, string email);
    }
}