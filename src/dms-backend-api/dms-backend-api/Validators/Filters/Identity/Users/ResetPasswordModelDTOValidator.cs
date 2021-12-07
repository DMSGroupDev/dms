using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity.Users
{
    public partial class ResetPasswordModelDTOValidator : AbstractValidator<ResetPasswordModelDTO>
    {
        public ResetPasswordModelDTOValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Code).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(8, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
        }
    }
}
