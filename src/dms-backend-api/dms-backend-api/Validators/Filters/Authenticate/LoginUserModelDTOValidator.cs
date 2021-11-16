
using dms_backend_api.ExternalModel.Authenticate;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class LoginUserModelDTOValidator : AbstractValidator<LoginUserModelDTO>
    {
        public LoginUserModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).EmailAddress().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(5, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
        }
    }
}