using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateRoleModelDTOValidator : AbstractValidator<UpdateRoleModelDTO>
    {
        public UpdateRoleModelDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
            RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
