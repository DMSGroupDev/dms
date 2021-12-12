
using Dapper;
using dms_backend_api.Data;
using dms_backend_api.Domain;
using dms_backend_api.Domain.Identity;
using dms_backend_api.Domain.Identity.Domain;
using dms_backend_api.Helpers;
using dms_backend_api.Model;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace dms_backend_api.Services
{
    public partial class IdentityService : IIdentityService
    {

        #region Fields
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<IdentityService> _logger;
        #endregion

        #region Ctor
        public IdentityService(RoleManager<ApplicationRole> roleManager,
                               UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               ApplicationDbContext applicationDbContext,
                               ILogger<IdentityService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }
        #endregion

        #region Methods

        public RoleManager<ApplicationRole> GetRoleManager()
        {
            return _roleManager;
        }

        public UserManager<ApplicationUser> GetUserManager()
        {
            return _userManager;
        }

        public SignInManager<ApplicationUser> GetSignInManager()
        {
            return _signInManager;
        }

        #region Domains

        public async Task<bool> ValidationRegisterDomainAsync(Domains validateDomain)
        {
            try
            {
                var iConn = _applicationDbContext.Database.GetDbConnection();
                if (iConn.State != ConnectionState.Open)
                    iConn.Open();

                await iConn.OpenAsync();

                return ((await iConn.QueryAsync<bool?>("ValidateRegisterDomain",
                                new { validateDomain.DomainName },
                                commandType: CommandType.StoredProcedure)).FirstOrDefault() ?? false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidationRegisterDomainAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<DbResult> RegisterDomainAsync(Domains registerDomains, Guid ownerId)
        {
            try
            {
                var iConn = _applicationDbContext.Database.GetDbConnection();
                if (iConn.State != ConnectionState.Open)
                    iConn.Open();

                await iConn.OpenAsync();

                return ((await iConn.QueryAsync<DbResult?>("RegisterDomain",
                                 new
                                 {
                                     registerDomains.DomainName,
                                     ownerId
                                 },
                                 commandType: CommandType.StoredProcedure)).FirstOrDefault() ?? DbResult.Failed());
            }
            catch (Exception ex)
            {
                _logger.LogError($"RegisterDomainAsync: {ex.Message}");
                return DbResult.Failed();
            }
        }

        public async Task<DbResult> AddUserToDomainAsync(Domains registerDomains, string email)
        {
            try
            {
                var iConn = _applicationDbContext.Database.GetDbConnection();
                if (iConn.State != ConnectionState.Open)
                    iConn.Open();

                await iConn.OpenAsync();
                var user = _userManager.Users.Where(x => x.Email == email).FirstOrDefault();
                if (user is not null)
                    return ((await iConn.QueryAsync<DbResult?>("AddToDomain",
                                     new
                                     {
                                         registerDomains.DomainName,
                                         userId = user.Id,
                                     },
                                     commandType: CommandType.StoredProcedure)).FirstOrDefault() ?? DbResult.Failed());

                return DbResult.Failed(new ErrorModel() { AttemptedValue = email, ErrorCode = (int)ErrorCodes.NotFound, PropertyName = "Email" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddToDomainAsync: {ex.Message}");
                return DbResult.Failed();
            }
        }
        #endregion

        #endregion
    }
}