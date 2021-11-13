using dms_backend_api.Domain.Identity;
using dms_backend_api.Response;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dms_backend_api.Services.Identity
{
    public interface IEmailSender
    {
        Task<BasicResponse> SendEmailAsync(ApplicationUser emailTo, ApplicationUser? emailFrom = null, string? subject = null, string? htmlMessage = null, string? templateId = null, object? templateData = null, IList<Attachment>? attachments = null);
    }
}
