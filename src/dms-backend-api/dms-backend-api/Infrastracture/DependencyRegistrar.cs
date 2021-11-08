
using AutoMapper.Data;
using dms_backend_api.ExternalModel.Authenticate;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.ExternalModel.Util;
using dms_backend_api.Factories;
using dms_backend_api.Mapping;
using dms_backend_api.Services;
using dms_backend_api.Services.Identity;
using dms_backend_api.Services.Utils;
using dms_backend_api.Validators.Filters;
using dms_backend_api.Validators.Filters.Identity;
using dms_backend_api.Validators.Filters.Util;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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

            #region Mapping

            services.AddAutoMapper(mc =>
            {
                mc.AddDataReaderMapping();
                mc.AddProfile(new MappingProfile());
            });

            #endregion

            #region Utils
            services.AddSendGrid(options => { options.ApiKey = configuration["SENDGRID_API_KEY"]; });
            services.AddHttpContextAccessor();
            #endregion

            #region Validation
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
            .AddMvc(x => x.Filters.Add(new ValidationFilter())).AddFluentValidation(config => {config.AutomaticValidationEnabled = true; });

            services.AddTransient<IValidator<LoginUserModelDTO>, LoginUserModelDTOValidator>();
            services.AddTransient<IValidator<RegisterUserModelDTO>, RegisterUserModelDTOValidator>();
            services.AddTransient<IValidator<UserValidationModelDTO>, UserValidationModelDTOValidator>();
            services.AddTransient<IValidator<AddRoleModelDTO>, AddRoleModelDTOValidator>();
            services.AddTransient<IValidator<UpdateRoleModelDTO>, UpdateRoleModelDTOValidator>();
            services.AddTransient<IValidator<MailModelDTO>, MailModelDTOValidator>();
            #endregion

            #region Factories
            services.AddSingleton<IErrorFactory, ErrorFactory>();
            #endregion

            #region Services
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IEmailSender, SendgridService>();
            #endregion
        }
    }
}
