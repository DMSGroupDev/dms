using AutoMapper;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Factories;
using dms_backend_api.Helpers;
using dms_backend_api.Model;
using dms_backend_api.Response;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace dms_backend_api.Controllers
{
    [ApiController]
    [Route("/api/identity/[action]")]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(BasicResponse))]
    public partial class IdentityController : ControllerBase
    {
        #region Fields

        private readonly ILogger<IdentityController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IErrorFactory _errorFactory;
        private readonly IUserTwoFactorTokenProvider<ApplicationUser> _tokenProvider;

        #endregion

        #region Ctor

        public IdentityController(IIdentityService identityService,
                                      ILogger<IdentityController> logger,
                                      IMapper mapper,
                                      IEmailSender emailSender,
                                      IErrorFactory errorFactory,
                                      IUserTwoFactorTokenProvider<ApplicationUser> tokenProvider,
                                       SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = identityService.GetUserManager();
            _roleManager = identityService.GetRoleManager();
            _mapper = mapper;
            _emailSender = emailSender;
            _errorFactory = errorFactory;
            _tokenProvider = tokenProvider;
            _signInManager = signInManager;
        }

        #endregion

        #region Methods

        #region UserIdentityFunctions

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - User Functions" })]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmationEmailModelDTO confirmationEmailModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(confirmationEmailModel.UserId);
                    if (user is null)
                        return NotFound(new BasicResponse()
                        {
                            Message = $"Unable to find User with id : {confirmationEmailModel.UserId}",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = confirmationEmailModel.UserId,
                                ErrorCode = (int) ErrorCodes.NotFound,
                                PropertyName = "UserId",
                                ErrorMessage = $"Unable to find User with id: {confirmationEmailModel.UserId}"
                            }
                        }
                            }
                        });

                    var result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationEmailModel.Code)));
                    if (result.Succeeded)
                    {
                        return Ok(new BasicResponse()
                        {
                            Message = $"User confirmed sucessfully.",
                            StatusCode = (int)HttpStatusCode.OK
                        });
                    }

                    foreach (var err in result.Errors)
                        ModelState.AddModelError(err.Code, err.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ConfirmEmailAsync: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }

            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - User Functions" })]
        public async Task<IActionResult> ReConfirmEmailAsync(ReConfirmationEmailModelDTO reconfirmationEmailModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(reconfirmationEmailModel.Email);
                    if (user is null)
                        return NotFound(new BasicResponse()
                        {
                            Message = $"Unable to find User with email : {reconfirmationEmailModel.Email}",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = reconfirmationEmailModel.Email,
                                ErrorCode = (int) ErrorCodes.NotFound,
                                PropertyName = "Email",
                                ErrorMessage = $"Unable to find User with id: {reconfirmationEmailModel.Email}"
                            }
                            }
                            }
                        });
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var emailResponse = await _emailSender.SendEmailAsync(emailTo: user, subject: "Confirm your email", htmlMessage: $"Please confirm your account by <a href='" +
                        $"{HtmlEncoder.Default.Encode(string.Format(reconfirmationEmailModel.RegistrationCallbackUrl, user.Id, code))}'>clicking here</a>.");

                    if (emailResponse.StatusCode == (int)HttpStatusCode.OK)
                        return Ok(new RegisterReponse() { ConfirmationCode = code, UserId = user.Id.ToString(), Message = $"Confirmation code:{code} ", StatusCode = (int)HttpStatusCode.OK });
                    return Ok(emailResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReConfirmEmailAsync: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }


        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - User Functions" })]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordModelDTO forgetPasswordModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(forgetPasswordModel.Email);
                    if (user is null)
                        return NotFound(new BasicResponse()
                        {
                            Message = $"Unable to find User with email  : {forgetPasswordModel.Email}",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = forgetPasswordModel.Email,
                                ErrorCode = (int) ErrorCodes.NotFound,
                                PropertyName = "Email",
                                ErrorMessage = $"Unable to find User with id: {forgetPasswordModel.Email}"
                            }
                            }
                            }
                        });

                    if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return NotFound(new BasicResponse()
                        {
                            Message = $"Unable to valid User with email: {forgetPasswordModel.Email}",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = forgetPasswordModel.Email,
                                ErrorCode = (int) ErrorCodes.AlreadyConfirmed,
                                PropertyName = "Email",
                                ErrorMessage = $"Unable to valid User with email: {forgetPasswordModel.Email}"
                            }
                            }
                            }
                        });
                    }
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var emailResponse = await _emailSender.SendEmailAsync(emailTo: user, subject: "Reset Password", htmlMessage: $"Please reset your password by <a href='" +
                        $"{HtmlEncoder.Default.Encode(string.Format(forgetPasswordModel.ForgetPasswordCallbackUrl, user.Id, code))}'>clicking here</a>.");

                    if (emailResponse.StatusCode == (int)HttpStatusCode.OK)
                        return Ok(new ForgetPasswordResponse() { ResetCode = code, Message = $"Reset code:{code} ", StatusCode = (int)HttpStatusCode.OK });

                    return Ok(emailResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ForgetPassword: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - User Functions" })]
        public async Task<IActionResult> ValidateForgetPasswordAsync(ValidateForgetPasswordModelDTO validateForgetPasswordModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(validateForgetPasswordModel.UserId.ToString());
                    if (user is not null)
                    {
                        if (await _tokenProvider.ValidateAsync("ResetPassword", System.Text.Encoding.Default.GetString(WebEncoders.Base64UrlDecode(validateForgetPasswordModel.Code)), _userManager, user))
                            return Ok(new BasicResponse() { Message = $"Combination is valid", StatusCode = (int)HttpStatusCode.OK });
                        else
                            return BadRequest(new BasicResponse() { Message = $"Validation failed", StatusCode = (int)HttpStatusCode.BadRequest });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidateForgetPassword: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - User Functions" })]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModelDTO resetPasswordModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString());
                    if (user is null)
                        return NotFound(new BasicResponse()
                        {
                            Message = $"Unable to find User with id : {resetPasswordModel.UserId}",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = resetPasswordModel.UserId.ToString(),
                                ErrorCode = (int) ErrorCodes.NotFound,
                                PropertyName = "UserId",
                                ErrorMessage = $"Unable to find User with id: {resetPasswordModel.UserId}"
                            }
                            }
                            }
                        });

                    var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, resetPasswordModel.Password, false);
                    if (passwordCheck.Succeeded || passwordCheck.IsLockedOut)
                    {
                        return Ok(new BasicResponse()
                        {
                            Message = $"Password is already used.",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            ErrorResponse = new ErrorResponse()
                            {
                                Errors = new List<ErrorModel> {
                            new ErrorModel()
                            {
                                AttemptedValue = resetPasswordModel.Password,
                                ErrorCode = (int) ErrorCodes.NotUnique,
                                PropertyName = "Password",
                                ErrorMessage = $"Password is already used."
                            }
                            }
                            }
                        });
                    }
                    var result = await _userManager.ResetPasswordAsync(user, System.Text.Encoding.Default.GetString(WebEncoders.Base64UrlDecode(resetPasswordModel.Code)), resetPasswordModel.Password);

                    if (result.Succeeded)
                    {
                        return Ok(new BasicResponse()
                        {
                            Message = $"Password reset sucesfully.",
                            StatusCode = (int)HttpStatusCode.OK
                        });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ResetPassword: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        #endregion

        #region Users

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Users" })]
        public async Task<IActionResult> AddUser([FromBody] AddUserModelDTO addUserModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userManager.CreateAsync(_mapper.Map<ApplicationUser>(addUserModel));

                    if (result.Succeeded)
                    {
                        var user = _userManager.Users.Where(x => x.Email == addUserModel.Email).FirstOrDefault();
                        if (user is not null)
                            return Ok(new BasicResponse() { Message = $"User sucessfully created:{ user.Id}", StatusCode = (int)HttpStatusCode.OK });

                        return NotFound(new BasicResponse() { Message = $"User not found :{ addUserModel.Email}", StatusCode = (int)HttpStatusCode.NotFound });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddUser: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.ExpectationFailed });
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Identity - Users" })]
        public IActionResult GetUserById(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _userManager.Users.Where(x => x.Id.Equals(Id)).First();
                    if (role is not null)
                        return Ok(role);
                    return NotFound(new BasicResponse() { Message = $"User with id:{Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetUserById: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Identity - Users" })]
        public async Task<IActionResult> DeleteUserByIdAsync(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var user = _userManager.Users.Where(x => x.Id.Equals(Id)).First();
                    if (user is not null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            return Ok(new BasicResponse() { Message = $"User sucessfully deleted:{ user.Id}", StatusCode = (int)HttpStatusCode.OK });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return NotFound(new BasicResponse() { Message = $"User with id:{Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteUserByIdAsync: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        /* [HttpGet]
         public async Task<IActionResult> DeleteUserUsernamedAsync(string username)
         {
             try
             {

                 var user = _userManager.Users.Where(x => x.UserName.Equals(username)).First();
                 if (user != null)
                 {
                     var result = await _userManager.DeleteAsync(user);
                     if (result.Succeeded)
                     {
                         return Ok(new BasicResponse() { Message = $"User sucessfully deleted:{ user.Id}", StatusCode = (int)HttpStatusCode.OK });
                     }
                     foreach (var error in result.Errors)
                     {
                         ModelState.AddModelError(string.Empty, error.Description);
                     }
                 }
                 return NotFound(new BasicResponse() { Message = $"User with id:{username} was not found.", StatusCode = (int)HttpStatusCode.NotFound });

             }
             catch (Exception ex)
             {
                 _logger.LogError($"DeleteUserByIdAsync: {ex.Message}");
                 return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
             }
         }*/

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Users" })]
        public async Task<IActionResult> UpdateUseryIdAsync([FromBody] UpdateUserModelDTO updateUserModel)
        {
            try
            {
                if (updateUserModel.Id != Guid.Empty)
                {
                    var user = _userManager.Users.Where(x => x.Id.Equals(updateUserModel.Id)).First();
                    if (user is not null)
                    {
                        user.UserName = updateUserModel.UserName;
                        user.FirstName = updateUserModel.FirstName;
                        user.LastName = updateUserModel.LastName;

                        await _userManager.ChangePasswordAsync(user, updateUserModel.OldPassword, updateUserModel.Password);

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return Ok(new BasicResponse() { Message = $"User sucessfully updated:{user.Id}", StatusCode = (int)HttpStatusCode.OK });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return NotFound(new BasicResponse() { Message = $"User with id:{updateUserModel.Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateUseryIdAsync :{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Users" })]
        public async Task<IActionResult> ChangeUserPasswordAsync([FromBody] ChangeUserPasswordModelDTO updateUserModel)
        {
            try
            {
                if (updateUserModel.Id != Guid.Empty)
                {
                    var user = _userManager.Users.Where(x => x.Id.Equals(updateUserModel.Id)).First();
                    if (user is not null)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, updateUserModel.OldPassword, updateUserModel.Password); ;
                        if (result.Succeeded)
                        {
                            return Ok(new BasicResponse() { Message = $"User sucessfully updated:{user.Id}", StatusCode = (int)HttpStatusCode.OK });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return NotFound(new BasicResponse() { Message = $"User with id:{updateUserModel.Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ChangeUserPasswordAsync :{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        #endregion

        #region Roles

        [HttpGet]
        public string Echo()
        {
            return "Echo";
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Roles" })]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModelDTO addRoleModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _roleManager.CreateAsync(_mapper.Map<ApplicationRole>(addRoleModel));

                    if (result.Succeeded)
                    {
                        return Ok(new BasicResponse() { Message = $"Role sucessfully created:{ addRoleModel.Name}", StatusCode = (int)HttpStatusCode.OK });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddRole: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Identity - Roles" })]
        public IActionResult GetAllRoles()
        {
            try
            {
                return Ok(_roleManager.Roles.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllRoles: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Identity - Roles" })]
        public IActionResult GetRoleById(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(Id)).First();
                    if (role is not null)
                        return Ok(role);
                    return NotFound(new BasicResponse() { Message = $"Role with id:{Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetRoleById: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Identity - Roles" })]
        public async Task<IActionResult> DeleteRoleByIdAsync(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(Id)).First();
                    if (role is not null)
                    {
                        var result = await _roleManager.DeleteAsync(role);
                        if (result.Succeeded)
                        {
                            return Ok(new BasicResponse() { Message = $"Role sucessfully deleted:{ role.Name}", StatusCode = (int)HttpStatusCode.OK });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return NotFound(new BasicResponse() { Message = $"Role with id:{Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteRoleByIdAsync: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Roles" })]
        public async Task<IActionResult> UpdateRoleByIdAsync([FromBody] UpdateRoleModelDTO updateRoleModel)
        {
            try
            {
                if (updateRoleModel.Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(updateRoleModel.Id)).First();
                    if (role is not null)
                    {
                        role.Name = updateRoleModel.Name;
                        role.NormalizedName = updateRoleModel.NormalizedName;

                        var result = await _roleManager.UpdateAsync(role);
                        if (result.Succeeded)
                        {
                            return Ok(new BasicResponse() { Message = $"Role sucessfully updated:{role.Name}", StatusCode = (int)HttpStatusCode.OK });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return NotFound(new BasicResponse() { Message = $"Role with id:{updateRoleModel.Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateRoleByIdAsync :{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        #endregion

        #region Domain

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Domain" })]
        public IActionResult ValidationRegisterDomainAsync(ValidateRegisterDomainModelDTO validateRegisterDomainModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    /*Validation*/
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidationRegisterDomainAsync: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Domain" })]
        public IActionResult RegisterDomain(RegisterDomainModelDTO registerDomainModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RegisterDomain: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Domain" })]
        public IActionResult InviteUserToDomain(InviteToDomainModelDTO inviteToDomainModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"InviteUserToDomain: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Identity - Domain" })]
        public IActionResult AddUserToDomain(AddToDomainModelDTO addToDomainModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddUserToDomain: {ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"", StatusCode = (int)HttpStatusCode.BadRequest, ErrorResponse = _errorFactory.ModelStateToErrorResponse(ModelState) });
        }
        #endregion

        #endregion
    }
}
