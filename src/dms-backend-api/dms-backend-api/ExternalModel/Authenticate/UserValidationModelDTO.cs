using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Authenticate
{
    public partial class UserValidationModelDTO
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

    }
}
