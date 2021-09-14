
using dms_backend_api.Model;

namespace dms_backend_api.Response;
public class ErrorResponse
{
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
}
