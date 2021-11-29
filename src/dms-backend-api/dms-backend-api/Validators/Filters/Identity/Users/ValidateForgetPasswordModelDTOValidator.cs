using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class ValidateForgetPasswordModelDTOValidator : AbstractValidator<ValidateForgetPasswordModelDTO>
    {
        public ValidateForgetPasswordModelDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Code).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
