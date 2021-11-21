using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class ChangeUserPasswordModelDTO
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
