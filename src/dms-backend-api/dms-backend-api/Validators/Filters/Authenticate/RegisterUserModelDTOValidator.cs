
using dms_backend_api.ExternalModel.Authenticate;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class RegisterUserModelDTOValidator : AbstractValidator<RegisterUserModelDTO>
    {
        public RegisterUserModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Password).Length(5, 100);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).Length(5, 100);
        }
    }
}