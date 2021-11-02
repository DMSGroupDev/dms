
using Microsoft.AspNetCore.Identity;
using System;

namespace dms_backend_api.Domain.Identity
{
    public partial class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}