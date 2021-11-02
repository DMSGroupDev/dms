using dms_backend_api.Domain.Identity;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;

namespace dms_backend_api.ExternalModel.Util
{
    public partial class MailModelDTO
    {
        public ApplicationUser EmailTo { get; set; } = null!;
        public ApplicationUser? EmailFrom { get; set; } = null;
        public string? Subject { get; set; } = null;
        public string? HtmlMessage { get; set; } = null;
        public string? TemplateId { get; set; } = null;
        public object? TemplateData { get; set; } = null;
        public IList<Attachment>? Attachments { get; set; } = null;
    }
}
