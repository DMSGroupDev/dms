using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class ConfirmationEmailModelDTOValidator : AbstractValidator<ConfirmationEmailModelDTO>
    {
        public ConfirmationEmailModelDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Code).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
