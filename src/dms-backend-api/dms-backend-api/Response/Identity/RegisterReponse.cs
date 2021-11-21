
namespace dms_backend_api.Response
{
    public partial class RegisterReponse : BasicResponse
    {
        public string? ConfirmationCode { get; set; } = null!;
        public string? UserId { get; set; } = null!;
    }
}