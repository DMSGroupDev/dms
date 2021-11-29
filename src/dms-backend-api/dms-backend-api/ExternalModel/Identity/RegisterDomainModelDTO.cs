namespace dms_backend_api.ExternalModel.Identity
{
    public record ValidateRegisterDomainModelDTO(string DomainName);
    public record RegisterDomainModelDTO(string DomainName, string Email);
    public record InviteToDomainModelDTO(int DomainId, string EmailsToInvite, string Subject, string HtmlInviteMessage);
    public record AddToDomainModelDTO(int DomainId, string Email);
}
