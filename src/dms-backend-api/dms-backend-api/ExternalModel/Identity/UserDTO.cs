using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public record AddUserModelDTO(string UserName, string FirstName, string LastName, string Email, string Password);
    public record UpdateUserModelDTO(Guid Id, string UserName, string FirstName, string LastName, string Email, string Password, string OldPassword);
}
