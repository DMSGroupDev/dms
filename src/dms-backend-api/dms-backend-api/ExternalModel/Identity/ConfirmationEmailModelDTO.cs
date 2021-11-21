namespace dms_backend_api.ExternalModel.Identity
{
    public record ConfirmationEmailModelDTO(string UserId, string Code);
    public record ReConfirmationEmailModelDTO(string Email, string RegistrationCallbackUrl);
}
