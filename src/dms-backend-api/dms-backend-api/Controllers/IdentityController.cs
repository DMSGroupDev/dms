using AutoMapper;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.Factories;
using dms_backend_api.Response;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dms_backend_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/identity/[action]")]
    public partial class IdentityController : ControllerBase
    {
        #region Fields
        private readonly IIdentityService _identityService;
        private readonly ILogger<IdentityController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IErrorFactory _errorFactory;
        #endregion

        #region Ctor
        public IdentityController(IIdentityService identityService,
                                      ILogger<IdentityController> logger,
                                      IHttpContextAccessor httpContextAccessor,
                                      IMapper mapper,
                                      ITokenService tokenService,
                                      IEmailSender emailSender,
                                      IErrorFactory errorFactory)
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
            _errorFactory = errorFactory;
        }
        #endregion

        #region Methods

        #region Users

        [HttpPost]
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
                        if (user != null)
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

        [HttpGet]
        public IActionResult GetUserById(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _userManager.Users.Where(x => x.Id.Equals(Id)).First();
                    if (role != null)
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

        [HttpGet]
        public async Task<IActionResult> DeleteUserByIdAsync(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var user = _userManager.Users.Where(x => x.Id.Equals(Id)).First();
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

        [HttpPost]
        public async Task<IActionResult> UpdateUseryIdAsync([FromBody] UpdateUserModelDTO updateRoleModel)
        {
            try
            {
                if (updateRoleModel.Id != Guid.Empty)
                {
                    var user = _userManager.Users.Where(x => x.Id.Equals(updateRoleModel.Id)).First();
                    if (user != null)
                    {
                        user = _mapper.Map<ApplicationUser>(updateRoleModel);

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
                    return NotFound(new BasicResponse() { Message = $"User with id:{updateRoleModel.Id} was not found.", StatusCode = (int)HttpStatusCode.NotFound });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateUseryIdAsync :{ex.Message}");
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

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] AddRoleModelDTO addRoleModel)
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

        [HttpGet]
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

        [HttpGet]
        public IActionResult GetRoleById(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(Id)).First();
                    if (role != null)
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

        [HttpGet]
        public async Task<IActionResult> DeleteRoleByIdAsync(Guid Id)
        {
            try
            {
                if (Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(Id)).First();
                    if (role != null)
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

        [HttpPost]
        public async Task<IActionResult> UpdateRoleByIdAsync([FromBody] UpdateRoleModelDTO updateRoleModel)
        {
            try
            {
                if (updateRoleModel.Id != Guid.Empty)
                {
                    var role = _roleManager.Roles.Where(x => x.Id.Equals(updateRoleModel.Id)).First();
                    if (role != null)
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

        #endregion
    }
}
