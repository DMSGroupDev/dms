using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity.Domain
{
    public class ValidateRegisterDomainModelDTOValidator : AbstractValidator<ValidateRegisterDomainModelDTO>
    {
        public ValidateRegisterDomainModelDTOValidator()
        {
            RuleFor(x => x.DomainName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
