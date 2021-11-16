using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Authenticate;
using dms_backend_api.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class UserValidationModelDTOValidator : AbstractValidator<UserValidationModelDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserValidationModelDTOValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Email).MustAsync(async (Email, cancellation) =>
            {
                return await _userManager.FindByEmailAsync(email: Email) == null;
            }).WithMessage("Email must be unique").WithErrorCode(ErrorCodes.NotUnique.ToString());

            RuleFor(x => x.UserName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.UserName).MustAsync(async (UserName, cancellation) =>
            {
                return await _userManager.FindByNameAsync(UserName) == null;
            }).WithMessage("UserName must be unique").WithErrorCode(ErrorCodes.NotUnique.ToString());

        }
    }
}