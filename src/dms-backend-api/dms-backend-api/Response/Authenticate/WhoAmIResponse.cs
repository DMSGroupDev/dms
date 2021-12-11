using dms_backend_api.Domain.Identity;
using System.Collections.Generic;

namespace dms_backend_api.Response.Authenticate
{
    public class WhoAmIResponse
    {
        public WhoAmIResponse()
        {
            roles = new List<string?>();
        }
        public ApplicationUser? user { get; set; }
        public List<string?> roles { get; set; }
    }
}
