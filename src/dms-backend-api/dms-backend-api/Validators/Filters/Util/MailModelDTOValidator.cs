using dms_backend_api.ExternalModel.Util;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Util
{
    public class MailModelDTOValidator : AbstractValidator<MailModelDTO>
    {
        public MailModelDTOValidator()
        {
            RuleFor(x => x.EmailTo.Email).NotEmpty();
        }
    }
}
