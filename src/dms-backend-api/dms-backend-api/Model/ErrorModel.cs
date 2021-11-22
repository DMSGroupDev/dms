
using dms_backend_api.Helpers;
using FluentValidation.Results;

namespace dms_backend_api.Model
{
    public partial class ErrorModel
    {
        public string? PropertyName { get; set; } = null;
        public string? ErrorMessage { get; set; } = null;
        public object? AttemptedValue { get; set; } = null;
        public int? ErrorCode { get; set; }
        public ErrorModel() { }
        public ErrorModel(ValidationFailure error)
        {
            PropertyName = error.PropertyName;
            ErrorMessage = error.ErrorMessage;
            AttemptedValue = error.AttemptedValue;
            if (System.Enum.TryParse(error.ErrorCode, true, out ErrorCodes errorCode))
                ErrorCode = (int)errorCode;
            else
                ErrorCode = -1;
        }
    }
}