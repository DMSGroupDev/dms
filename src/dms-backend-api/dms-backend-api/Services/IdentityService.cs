
using dms_backend_api.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace dms_backend_api.Services
{
    public partial class IdentityService : IIdentityService
    {

        #region Fields
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Ctor
        public IdentityService(RoleManager<ApplicationRole> roleManager,
                               UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
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

        #region Register

        #endregion

        #region Login
        #endregion

        #endregion
    }
}