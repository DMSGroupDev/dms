using dms_backend_api.ExternalModel.Authenticate;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class UserValidationModelDTOValidator : AbstractValidator<UserValidationModelDTO>
    {
        public UserValidationModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.UserName).NotEmpty();
        }
    }
}