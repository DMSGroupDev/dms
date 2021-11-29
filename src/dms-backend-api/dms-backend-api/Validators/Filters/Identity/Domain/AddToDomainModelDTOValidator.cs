using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity.Domain
{
    public class AddToDomainModelDTOValidator : AbstractValidator<AddToDomainModelDTO>
    {
        public AddToDomainModelDTOValidator()
        {
            RuleFor(x => x.DomainId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
