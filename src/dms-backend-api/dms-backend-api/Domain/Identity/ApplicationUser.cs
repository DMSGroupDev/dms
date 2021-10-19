
using Microsoft.AspNetCore.Identity;
using System;

namespace dms_backend_api.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}