using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class AddRoleModelDTO
    {
        public string Name { get; set; } = null!;
        private string normalizedName = null!;
        public string NormalizedName
        {
            get => normalizedName;
            set => normalizedName = Name.ToUpper().Trim();
        }
        public int Priority { get; set; }
    }
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
    public record AssignUserToRoleModelDTO(Guid UserId, Guid RoleId);
}
