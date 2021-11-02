using AutoMapper;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Response;
using dms_backend_api.Response.Identity;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace dms_backend_api.Controllers
{
    [ApiController]
    [Route("/api/authenticate/[action]")]
    public partial class AuthenticateController : Controller
    {
        #region Fields
        private readonly IIdentityService _identityService;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        #endregion

        #region Ctor
        public AuthenticateController(IIdentityService identityService,
                                      ILogger<AuthenticateController> logger,
                                      IHttpContextAccessor httpContextAccessor,
                                      IMapper mapper,
                                      ITokenService tokenService,
                                      IEmailSender emailSender)
        {
            _identityService = identityService;
            _logger = logger;
            _signInManager = identityService.GetSignInManager();
            _userManager = identityService.GetUserManager();
            _roleManager = identityService.GetRoleManager();
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }
        #endregion

        #region Methods
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModelDTO registerUserModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<ApplicationUser>(registerUserModel);
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    var result = await _userManager.CreateAsync(user, registerUserModel.Password);
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        await _emailSender.SendEmailAsync(emailTo: user, subject: "Confirm your email", htmlMessage: $"Please confirm your account by <a href='" +
                            $"{HtmlEncoder.Default.Encode(string.Format(registerUserModel.RegistrationCallbackUrl, code))}'>clicking here</a>.");

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
            return BadRequest(new BasicResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserModelDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (_httpContextAccessor.HttpContext.User != null)
                        {
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                            return Ok(new LoginResponse()
                            {
                                ApplicationUser = user,
                                Token = _tokenService.GenerateToken(user, (await _userManager.GetRolesAsync(user)).ToList()),
                                Message = $"User sucessfully logged.",
                                StatusCode = (int)HttpStatusCode.OK
                            });
                        }
                        return Ok(new LoginResponse() { Message = $"User account don't exist.", StatusCode = (int)HttpStatusCode.ExpectationFailed });
                    }
                    if (result.IsLockedOut)
                    {
                        return Ok(new LoginResponse() { Message = $"User account locked out.", StatusCode = (int)HttpStatusCode.ExpectationFailed });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new LoginResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new LoginResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new BasicResponse() { Message = $"User logged out", StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new LoginResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }
        #endregion
    }
}