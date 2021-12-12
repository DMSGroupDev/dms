using System;

namespace dms_backend_api.ExternalModel.Identity
{
    public record ForgetPasswordModelDTO(string Email, string ForgetPasswordCallbackUrl);
    public record ResetPasswordModelDTO(Guid UserId, string Code, string Password);
    public record ValidateForgetPasswordModelDTO(Guid UserId, string Code);
    public record ChangeUserPasswordModelDTO(Guid Id, string OldPassword, string Password);
}
