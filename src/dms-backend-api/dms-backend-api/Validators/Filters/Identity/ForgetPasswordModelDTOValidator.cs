using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class ForgetPasswordModelDTOValidator : AbstractValidator<ForgetPasswordModelDTO>
    {
        public ForgetPasswordModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.RegistrationCallbackUrl).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
