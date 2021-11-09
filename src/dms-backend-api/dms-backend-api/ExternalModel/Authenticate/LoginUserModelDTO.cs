using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Authenticate
{
    public partial class LoginUserModelDTO
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
