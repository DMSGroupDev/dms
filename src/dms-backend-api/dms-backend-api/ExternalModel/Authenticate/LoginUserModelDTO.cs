using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Authenticate
{
    public partial class LoginUserModelDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
