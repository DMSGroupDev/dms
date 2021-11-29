using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class ForgetPasswordModelDTOValidator : AbstractValidator<ForgetPasswordModelDTO>
    {
        public ForgetPasswordModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.ForgetPasswordCallbackUrl).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
