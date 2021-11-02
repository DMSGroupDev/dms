using dms_backend_api.ExternalModel.Util;
using dms_backend_api.Response;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace dms_backend_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/util/[action]")]
    public partial class UtilController : Controller
    {
        #region Fields
        private readonly ILogger<UtilController> _logger;
        private readonly IEmailSender _emailSender;
        #endregion

        #region Ctor
        public UtilController(ILogger<UtilController> logger,
                              IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }
        #endregion

        #region Methods

        [HttpPost]
        public IActionResult Log([FromBody] LogModelDTO logModel)
        {
            try
            {
                _logger.Log(logModel.LogType, logModel.LogMessage, logModel.LogParameters);
                return Ok(new BasicResponse() { Message = $"Log inserted.", StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync([FromBody] MailModelDTO emailModel)
        {
            try
            {
                var response = await _emailSender.SendEmailAsync(emailModel.EmailTo, emailModel.EmailFrom, emailModel.Subject, emailModel.HtmlMessage, emailModel.TemplateId, emailModel.TemplateData, emailModel.Attachments);
                return Ok(new BasicResponse() { Message = $"Mail sent.", StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }

        #endregion
    }
}
