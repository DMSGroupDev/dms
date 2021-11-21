using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class ChangeUserPasswordModelDTOValidator : AbstractValidator<ChangeUserPasswordModelDTO>
    {
        public ChangeUserPasswordModelDTOValidator()
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(5, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
            RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString()).Length(5, 100).WithErrorCode(ErrorCodes.NotEnoughtLenght.ToString());
        }
    }
}
