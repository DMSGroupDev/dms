using dms_backend_api.Domain.Identity;
using System.Collections.Generic;

namespace dms_backend_api.Response.Identity
{
    public partial class LoginResponse : BasicResponse
    {
        public LoginResponse()
        {
            ApplicationRoles = new List<string>();
        }
        public ApplicationUser? ApplicationUser { get; set; }
        public string? Token { get; set; }
        public List<string> ApplicationRoles { get; set; }
    }
}
