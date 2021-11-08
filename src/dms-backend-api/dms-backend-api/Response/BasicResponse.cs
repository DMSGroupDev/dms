
namespace dms_backend_api.Response
{
    public partial class BasicResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public ErrorResponse? ErrorResponse { get; set; } = null;
    }
}