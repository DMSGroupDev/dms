
using dms_backend_api.ExternalModel.Authenticate;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Identity
{
    public partial class LoginUserModelDTOValidator : AbstractValidator<LoginUserModelDTO>
    {
        public LoginUserModelDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithErrorCode("1").EmailAddress().WithErrorCode("1");
            RuleFor(x => x.Password).NotEmpty().WithErrorCode("1").Length(5, 100).WithErrorCode("3");
        }
    }
}