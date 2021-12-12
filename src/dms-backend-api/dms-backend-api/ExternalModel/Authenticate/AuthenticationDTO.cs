namespace dms_backend_api.ExternalModel.Authenticate
{
    public record LoginUserModelDTO(string? Email, string? Username, string Password);
    public record RegisterUserModelDTO(string RegistrationCallbackUrl = null!, string? UserName = null!, string? FirstName = null!, string? LastName = null!, string? Email = null!, string? Password = null!, string? ConfirmPassword = null!);
    public record UserValidationModelDTO(string UserName, string Email);
}
