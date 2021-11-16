using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class AddRoleModelDTOValidator : AbstractValidator<AddRoleModelDTO>
    {
        public AddRoleModelDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
