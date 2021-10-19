using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Response;
using dms_backend_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dms_backend_api.Controllers
{
    [ApiController]
    [Route("/api/authenticate")]
    public class AuthenticateController : ControllerBase
    {
        #region Fields
        private readonly IIdentityService _identityService;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Ctor
        public AuthenticateController(IIdentityService identityService,
                                      ILogger<AuthenticateController> logger)
        {
            _identityService = identityService;
            _logger = logger;
            _signInManager = identityService.GetSignInManager();
            _userManager = identityService.GetUserManager();
            _roleManager = identityService.GetRoleManager();
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModel registerUserModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = registerUserModel.Email,
                        Email = registerUserModel.Email,
                        FirstName = registerUserModel.FirstName,
                        LastName = registerUserModel.LastName
                    };

                    var result = await _userManager.CreateAsync(user, registerUserModel.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        return Ok(new BasicResponse() { Message = $"Confirmation code:{code}", StatusCode = (int)HttpStatusCode.OK });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest });
        }
        
    }
}