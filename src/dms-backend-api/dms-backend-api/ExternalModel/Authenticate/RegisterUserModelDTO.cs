
using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Authenticate
{
    public partial class RegisterUserModelDTO
    {

        public string RegistrationCallbackUrl { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
