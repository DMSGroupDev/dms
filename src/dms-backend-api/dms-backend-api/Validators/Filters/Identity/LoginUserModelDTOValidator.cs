
using dms_backend_api.ExternalModel.Identity;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class LoginUserModelDTOValidator : AbstractValidator<LoginUserModelDTO>
    {
        public LoginUserModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).Length(5, 100);
        }
    }
}