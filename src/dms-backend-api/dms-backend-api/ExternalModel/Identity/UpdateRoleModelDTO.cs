using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateRoleModelDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        private string normalizedName = null!;
        public string NormalizedName
        {
            get => normalizedName;
            set => normalizedName = Name.ToUpper().Trim();
        }
    }
}
