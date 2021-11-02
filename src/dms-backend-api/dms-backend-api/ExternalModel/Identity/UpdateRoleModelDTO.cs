using System;
using System.ComponentModel.DataAnnotations;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateRoleModelDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        private string normalizedName = null!;
        public string NormalizedName
        {
            get => normalizedName;
            set => normalizedName = Name.ToUpper().Trim();
        }
    }
}
