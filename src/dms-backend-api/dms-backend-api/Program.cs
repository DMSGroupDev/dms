using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using dms_backend_api.Infrastracture;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using FluentValidation.AspNetCore;
using dms_backend_api.Validators.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSerilog();

// Specifying the configuration for serilog
Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                .ReadFrom.Configuration(builder.Configuration) // connect serilog to our configuration folder
                .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                .WriteTo.Console()
                .CreateLogger(); //initialise the logger

Log.Logger.Information("Application Starting");

builder.Services.AddControllers();

new DependencyRegistrar().Register(builder.Services);

var config = builder.Configuration;

string? keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

if (keyVaultEndpoint is null)
    throw new InvalidOperationException("Store the Key Vault endpoint in a KEYVAULT_ENDPOINT environment variable.");
/*
    builder.Configuration.AddAzureKeyVault(new SecretClient(new Uri(keyVaultEndpoint),
        new ClientSecretCredential((string)config["AzureKeyVault:TenantId"], (string)config["AzureKeyVault:ClientId"], (string)config["AzureKeyVault:ClientSecretId"])),
        new KeyVaultSecretManager());
*/
builder.Services.AddMvc(x => x.Filters.Add(new ValidationFilter())).AddFluentValidation();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "dms_backend_api", Version = "v1" });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dms_backend_api v1"));
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
