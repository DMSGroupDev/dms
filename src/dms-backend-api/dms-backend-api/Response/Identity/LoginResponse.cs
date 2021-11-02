using dms_backend_api.Domain.Identity;

namespace dms_backend_api.Response.Identity
{
    public partial class LoginResponse
    {
        public ApplicationUser? ApplicationUser { get; set; }
        public string? Token { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
    }
}
