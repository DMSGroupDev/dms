
namespace dms_backend_api.Response
{
    public partial class ForgetPasswordResponse : BasicResponse
    {
        public string? ResetCode { get; set; } = null!;
    }
}