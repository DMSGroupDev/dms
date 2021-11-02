using FluentValidation;

namespace dms_backend_api.ExternalModel.Identity
{
    public partial class UpdateRoleModelDTOValidator : AbstractValidator<UpdateRoleModelDTO>
    {
        public UpdateRoleModelDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
