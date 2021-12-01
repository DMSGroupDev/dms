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
            if (manager is not null && user is not null)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        private string GenerateToken(ApplicationUser user, string purpose)
        {
            string secretString = (string)_configuration.GetValue(typeof(string), "JWTSecret");
            if (string.IsNullOrEmpty(secretString))
                throw new InvalidOperationException("The JWT Secret is empty.");

            return secretString + user.Email + purpose + user.Id + "|?t=" + ConvertToTimestamp(DateTime.UtcNow);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(GenerateToken(user, purpose));
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (long.TryParse(token.Substring(token.IndexOf("|?t=") + 4), out long parsedDate))
            {
                DateTime date = UnixTimeStampToDateTime(parsedDate);
                if (date > DateTime.UtcNow.AddHours(-24) && date <= DateTime.UtcNow)
                    return Task.FromResult(token == GenerateToken(user, purpose));
            }
            return Task.FromResult(false);
        }

        #region Timestamps
        
        internal static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)elapsedTime.TotalSeconds;
        }

        internal static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime;
        }
        
        #endregion
        
        #endregion
    }
}
