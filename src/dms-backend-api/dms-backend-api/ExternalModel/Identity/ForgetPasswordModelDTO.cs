namespace dms_backend_api.ExternalModel.Identity
{
    public record ForgetPasswordModelDTO(string Email, string RegistrationCallbackUrl);
    public record ResetPasswordModelDTO(string Email, string Code, string Password);
}
