using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity.Domain
{
    public class RegisterDomainModelDTOValidator : AbstractValidator<RegisterDomainModelDTO>
    {
        public RegisterDomainModelDTOValidator()
        {
            RuleFor(x => x.DomainName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.OwnerId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
