using dms_backend_api.Domain.Identity;
using System.Collections.Generic;

namespace dms_backend_api.Services.Identity
{
    public partial interface ITokenService
    {
        public string GenerateToken(ApplicationUser user, List<string> applicationRoles);
        /*public int? ValidateToken(string token);*/
    }
}
