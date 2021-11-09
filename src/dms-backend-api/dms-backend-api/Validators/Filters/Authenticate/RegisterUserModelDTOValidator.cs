
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Authenticate;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class RegisterUserModelDTOValidator : AbstractValidator<RegisterUserModelDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterUserModelDTOValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            RuleFor(x => x.Email).NotEmpty().WithErrorCode("1").EmailAddress().WithErrorCode("1");
            RuleFor(x => x.Email).MustAsync(async (Email, cancellation) =>
            {
                return await _userManager.FindByEmailAsync(email: Email) == null;
            }).WithMessage("Email must be unique").WithErrorCode("2");

            RuleFor(x => x.UserName).NotEmpty().WithErrorCode("1");
            RuleFor(x => x.UserName).MustAsync(async (UserName, cancellation) =>
            {
                return await _userManager.FindByNameAsync(UserName) == null;
            }).WithMessage("UserName must be unique").WithErrorCode("2");

            RuleFor(x => x.FirstName).NotEmpty().WithErrorCode("1");
            RuleFor(x => x.LastName).NotEmpty().WithErrorCode("1");
            RuleFor(x => x.Password).NotEmpty().WithErrorCode("1").Length(5, 100).WithErrorCode("3");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).Length(5, 100).WithErrorCode("4");
        }
    }
}