using dms_backend_api.Domain.Identity;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dms_backend_api.Infrastracture.Jobs
{
    public class RemoveUncompletedRegistrationJob : IRemoveUncompletedRegistrationJob
    {
        #region Fields
        private readonly ILogger<RemoveUncompletedRegistrationJob> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        #endregion

        #region Ctor
        public RemoveUncompletedRegistrationJob(
            ILogger<RemoveUncompletedRegistrationJob> logger,
            IIdentityService identityService)
        {
            _logger = logger;
            _identityService = identityService;
            _userManager = _identityService.GetUserManager();
        }
        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            try
            {
                var users = _userManager.Users.Where(x => x.EmailConfirmed == false && x.CreatedOnUtc <= DateTime.UtcNow.AddHours(-48)).ToList();
                foreach (var user in users)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RemoveUncompletedRegistration: {ex.Message}");
            }
        }

        #endregion
    }
}
