using dms_backend_api.Domain.Identity;
using dms_backend_api.Response;
using dms_backend_api.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
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
        public async Task<BasicResponse> SendEmailAsync(ApplicationUser emailTo, ApplicationUser? emailFrom = null, string? subject = null, string? htmlMessage = null, string? templateId = null, object? templateData = null, IList<Attachment>? attachments = null)
        {
            try
            {
                var msg = new SendGridMessage
                {
                    From = new EmailAddress()
                    {
                        Email = emailFrom?.Email ?? _configuration.GetValue("SendGrid:From", "test@example.com"),
                        Name = !string.IsNullOrWhiteSpace($"{emailFrom?.FirstName} {emailFrom?.LastName}") ? $"{emailFrom?.FirstName} {emailFrom?.LastName}" : _configuration.GetValue("SendGrid:FromName", "test@example.com")
                    },
                    Subject = subject
                };

                if (!string.IsNullOrEmpty(htmlMessage))
                    msg.HtmlContent = htmlMessage;

                if (!string.IsNullOrEmpty(templateId))
                {
                    msg.SetTemplateId(templateId);
                    msg.SetTemplateData(templateData);
                }

                if (attachments != null)
                    if (attachments.Count > 0)
                        msg.AddAttachments(attachments);

                msg.AddTo(new EmailAddress()
                {
                    Email = emailTo.Email ?? _configuration.GetValue("SendGrid:To", "test@example.com"),
                    Name = !string.IsNullOrWhiteSpace($"{emailTo?.FirstName} {emailTo?.LastName}") ? $"{emailTo?.FirstName} {emailTo?.LastName}" : "Example User",
                });

                if (_configuration.GetValue("SendGrid:SandboxMode", false))
                    msg.MailSettings = new MailSettings
                    {
                        SandboxMode = new SandboxMode
                        {
                            Enable = true
                        }
                    };

                var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                    return new BasicResponse() { Message = $"Mail sent.", StatusCode = (int)HttpStatusCode.OK };

                _logger.LogError($"SendEmailAsync :{response.Body}");
                return new BasicResponse() { Message = $"Problem with sending email {response.Body}.", StatusCode = (int)HttpStatusCode.NotAcceptable };
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendEmailSendgrid: {ex.Message}");
                return new BasicResponse();
            }
            #endregion
        }
    }
}
