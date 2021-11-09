
using FluentValidation.Results;

namespace dms_backend_api.Model
{
    public partial class ErrorModel
    {
        public string? PropertyName { get; set; } = null;
        public string? ErrorMessage { get; set; } = null;
        public object? AttemptedValue { get; set; } = null;
        public string? ErrorCode { get; set; } = null;
        public ErrorModel() { }
        public ErrorModel(ValidationFailure error)
        {
            PropertyName = error.PropertyName;
            ErrorMessage = error.ErrorMessage;
            AttemptedValue = error.AttemptedValue;
            ErrorCode = error.ErrorCode;
        }
    }
}