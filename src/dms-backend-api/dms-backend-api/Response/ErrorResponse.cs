
using dms_backend_api.Model;
using System.Collections.Generic;

namespace dms_backend_api.Response
{
    public partial class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}