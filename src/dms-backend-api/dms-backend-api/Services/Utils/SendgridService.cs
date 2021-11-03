using dms_backend_api.Domain.Identity;
using dms_backend_api.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dms_backend_api.Services.Utils
{
    public partial class SendgridService : IEmailSender
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<SendgridService> _logger;
        #endregion

        #region Ctor
        public SendgridService(IConfiguration configuration,
                               ISendGridClient sendGridClient,
                               ILogger<SendgridService> logger)
        {
            _configuration = configuration;
            _sendGridClient = sendGridClient;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task<SendGrid.Response?> SendEmailAsync(ApplicationUser emailTo, ApplicationUser? emailFrom = null, string? subject = null, string? htmlMessage = null, string? templateId = null, object? templateData = null, IList<Attachment>? attachments = null)
        {
            try
            {
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(emailFrom?.Email ?? _configuration.GetValue("SendGrid:From", "test@example.com"), $"{emailFrom?.FirstName} {emailFrom?.LastName}" ?? _configuration.GetValue("SendGrid:FromName", "test@example.com")),
                    Subject = subject
                };
                if (!string.IsNullOrEmpty(htmlMessage))
                    msg.PlainTextContent = htmlMessage;

                if (!string.IsNullOrEmpty(templateId))
                {
                    msg.SetTemplateId(templateId);
                    msg.SetTemplateData(templateData);
                }

                if (attachments != null)
                    if (attachments.Count > 0)
                        msg.AddAttachments(attachments);

                msg.AddTo(new EmailAddress(emailTo.Email ?? _configuration.GetValue("SendGrid:To", "test@example.com"), $"{emailTo.FirstName} {emailTo.LastName}" ?? "Example User"));

                if (_configuration.GetValue("SendGrid:SandboxMode", false))
                    msg.MailSettings = new MailSettings
                    {
                        SandboxMode = new SandboxMode
                        {
                            Enable = true
                        }
                    };
                return await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendEmailSendgrid: {ex.Message}");
                return null;
            }
            #endregion
        }
    }
}
