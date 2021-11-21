using dms_backend_api.Domain.Identity;
using dms_backend_api.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateUserModelDTOValidator : AbstractValidator<UpdateUserModelDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UpdateUserModelDTOValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.UserName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.UserName).MustAsync(async (UserName, cancellation) =>
            {
                return await _userManager.FindByNameAsync(UserName) == null;
            }).WithMessage("UserName must be unique").WithErrorCode(ErrorCodes.NotUnique.ToString());

            RuleFor(x => x.FirstName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.LastName).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.OldPassword).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(5, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
            RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(5, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
        }
    }
}
