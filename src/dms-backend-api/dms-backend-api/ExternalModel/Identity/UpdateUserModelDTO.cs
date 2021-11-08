using System;
using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateUserModelDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
}
