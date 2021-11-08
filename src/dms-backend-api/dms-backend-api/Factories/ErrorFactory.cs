using dms_backend_api.Model;
using dms_backend_api.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace dms_backend_api.Factories
{
    public partial class ErrorFactory : IErrorFactory
    {
        #region Fields
        #endregion

        #region Ctor
        public ErrorFactory()
        {

        }
        #endregion

        #region Methods
        public ErrorResponse ModelStateToErrorResponse(ModelStateDictionary modelState)
        {
            var errorResponse = new ErrorResponse();
            if (!modelState.IsValid && modelState.ErrorCount > 0)
            {
                var errorInModelState = modelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value?.Errors.Select(x => x.ErrorMessage)).ToList();

                foreach (var error in errorInModelState)
                {
                    if (error.Value != null)
                    {
                        foreach (var errorMessage in error.Value)
                            errorResponse.Errors.Add(new ErrorModel() { FieldName = error.Key, ErrorMessage = errorMessage });
                    }
                }
            }
            return errorResponse;
        }
        #endregion
    }
}