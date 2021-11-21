﻿using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateUserModelDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
