using Microsoft.Extensions.Configuration;
using System;

namespace dms_backend_api.Helpers
{
    public static class EnviromentVariablesHelper
    {
        #region Methods
        public static void EnviromentVariablesCheck(IConfigurationBuilder config)
        {
            string? keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
            if (keyVaultEndpoint is null)
                throw new InvalidOperationException("Store the Key Vault endpoint in a KEYVAULT_ENDPOINT environment variable.");

            string? jwtValidAudience = Environment.GetEnvironmentVariable("JWT_ValidAudience");
            if (jwtValidAudience is null)
                throw new InvalidOperationException("Store the JWT ValidAudience in a JWT_ValidAudience environment variable.");

            string? jwtValidIssuer = Environment.GetEnvironmentVariable("JWT_ValidIssuer");
            if (jwtValidIssuer is null)
                throw new InvalidOperationException("Store the JWT ValidIssuer in a JWT_ValidIssuer environment variable.");
        }
        #endregion
    }
}
