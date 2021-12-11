
using Microsoft.AspNetCore.Identity;
using System;

namespace dms_backend_api.Domain.Identity
{
    public partial class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime? CreatedOnUtc { get; set; } = null!;
        public int Priority { get; set; }
    }
}