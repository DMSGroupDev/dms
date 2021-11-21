using dms_backend_api.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace dms_backend_api.Services.Identity
{
    public class UserTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : ApplicationUser
    {
        #region Fields
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor

        public UserTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region Methods
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            if (manager != null && user != null)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        // Genereates a simple token based on the user id, email and another string.
        private string GenerateToken(ApplicationUser user, string purpose)
        {
            string secretString = (string)_configuration.GetValue(typeof(string), "JWTSecret");
            if (string.IsNullOrEmpty(secretString))
                throw new InvalidOperationException("The JWT Secret is empty.");

            return secretString + user.Email + purpose + user.Id;
        }

        public Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(GenerateToken(user, purpose));
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(token == GenerateToken(user, purpose));
        }
        #endregion
    }
}
