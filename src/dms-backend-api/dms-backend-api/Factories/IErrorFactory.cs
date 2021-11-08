using dms_backend_api.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dms_backend_api.Factories
{
    public interface IErrorFactory
    {
        ErrorResponse ModelStateToErrorResponse(ModelStateDictionary modelState);
    }
}
