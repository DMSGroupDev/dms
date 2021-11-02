
using AutoMapper.Data;
using dms_backend_api.Mapping;
using dms_backend_api.Services;
using dms_backend_api.Services.Identity;
using dms_backend_api.Services.Utils;
using dms_backend_api.Validators.Filters;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using System;

namespace dms_backend_api.Infrastracture
{

    public partial class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            /*Mapping*/
            services.AddAutoMapper(mc =>
            {
                mc.AddDataReaderMapping();
                mc.AddProfile(new MappingProfile());
            });

            /*Utils*/
            services.AddSendGrid(options => { options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? configuration["SendGrid:ApiKey"]; });
            services.AddHttpContextAccessor();

            /*Validator*/
            services.AddMvc(x => x.Filters.Add(new ValidationFilter())).AddFluentValidation();

            /*Services*/
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IEmailSender, SendgridService>();
        }
    }
}
