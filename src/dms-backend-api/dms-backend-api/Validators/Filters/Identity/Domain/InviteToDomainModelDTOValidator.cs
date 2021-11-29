using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity.Domain
{
    public class InviteToDomainModelDTOValidator : AbstractValidator<InviteToDomainModelDTO>
    {
        public InviteToDomainModelDTOValidator()
        {
            RuleFor(x => x.DomainId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.EmailsToInvite).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Subject).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.HtmlInviteMessage).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
