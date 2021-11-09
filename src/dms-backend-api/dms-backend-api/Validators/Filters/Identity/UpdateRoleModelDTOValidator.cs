using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateRoleModelDTOValidator : AbstractValidator<UpdateRoleModelDTO>
    {
        public UpdateRoleModelDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithErrorCode("1");
            RuleFor(x => x.Name).NotEmpty().WithErrorCode("1");
        }
    }
}
