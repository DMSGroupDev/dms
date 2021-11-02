using AutoMapper;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Identity;
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
        #endregion

        #region Ctor
        public IdentityController(IIdentityService identityService,
                                      ILogger<IdentityController> logger,
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

        #region Roles

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
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
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
                _logger.LogError($"{ex.Message}");
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
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
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
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
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
                _logger.LogError($"{ex.Message}");
                return BadRequest(new BasicResponse() { Message = $"{ex.Message}", StatusCode = (int)HttpStatusCode.BadRequest });
            }
            return BadRequest(new BasicResponse() { Message = $"{string.Join(Environment.NewLine, ModelState)}", StatusCode = (int)HttpStatusCode.ExpectationFailed });
        }
        #endregion

        #endregion
    }
}
