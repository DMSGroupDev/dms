
using dms_backend_api.ExternalModel.Identity;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class RegisterUserModelValidator : AbstractValidator<RegisterUserModel>
    {
        public RegisterUserModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Password).Length(5, 100);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).Length(5, 100);
        }
    }
}