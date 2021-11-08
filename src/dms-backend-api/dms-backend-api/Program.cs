using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using dms_backend_api.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace dms_backend_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    EnviromentVariablesHelper.EnviromentVariablesCheck(config);

                    var root = config.Build();
                    
                    config.AddEnvironmentVariables();

                    config.AddAzureKeyVault(new SecretClient(new Uri((string)root.GetValue(typeof(string), "KEYVAULT_ENDPOINT")),
                        new ClientSecretCredential((string)root["AzureKeyVault:TenantId"], (string)root["AzureKeyVault:ClientId"], (string)root["AzureKeyVault:ClientSecretId"])),
                        new KeyVaultSecretManager());

                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
