using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class ReConfirmationEmailModelDTOValidator : AbstractValidator<ReConfirmationEmailModelDTO>
    {
        public ReConfirmationEmailModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.RegistrationCallbackUrl).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());

        }
    }
}
