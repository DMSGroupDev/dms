using dms_backend_api.Domain.Identity;
using dms_backend_api.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class AssignUserToRoleModelDTOValidator : AbstractValidator<AssignUserToRoleModelDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AssignUserToRoleModelDTOValidator(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            RuleFor(x => x.RoleId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.RoleId).MustAsync(async (RoleId, cancellation) =>
            {
                return await _roleManager.FindByIdAsync(RoleId.ToString()) == null;
            }).WithMessage("Role not found").WithErrorCode(ErrorCodes.NotFound.ToString());

            RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.UserId).MustAsync(async (UserId, cancellation) =>
            {
                return await _userManager.FindByIdAsync(UserId.ToString()) == null;
            }).WithMessage("Role not found").WithErrorCode(ErrorCodes.NotFound.ToString());

        }
    }
}
