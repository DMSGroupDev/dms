using dms_backend_api.ExternalModel.Util;
using dms_backend_api.Helpers;
using FluentValidation;

namespace dms_backend_api.Validators.Filters.Util
{
    public class MailModelDTOValidator : AbstractValidator<MailModelDTO>
    {
        public MailModelDTOValidator()
        {
            RuleFor(x => x.EmailTo.Email).NotEmpty().WithErrorCode(ErrorCodes.EmptyOrInvalid.ToString());
        }
    }
}
