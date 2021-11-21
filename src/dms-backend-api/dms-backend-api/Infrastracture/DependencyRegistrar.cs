
using AutoMapper.Data;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Authenticate;
using dms_backend_api.ExternalModel.Identity;
using dms_backend_api.ExternalModel.Util;
using dms_backend_api.Factories;
using dms_backend_api.Mapping;
using dms_backend_api.Services;
using dms_backend_api.Services.Identity;
using dms_backend_api.Services.Utils;
using dms_backend_api.Validators;
using dms_backend_api.Validators.Filters;
using dms_backend_api.Validators.Filters.Identity;
using dms_backend_api.Validators.Filters.Util;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            #region Identity
            services.AddSingleton<IUserTwoFactorTokenProvider<ApplicationUser>, UserTokenProvider<ApplicationUser>>();
            #endregion

            #region Utils
            var SendgridApiKey = (string)configuration.GetValue(typeof(string), "SendgridApiKey");
            if (string.IsNullOrEmpty(SendgridApiKey))
                throw new InvalidOperationException("The SendgridApiKey is empty.");

            services.AddSendGrid(options => { options.ApiKey = SendgridApiKey; });
            services.AddHttpContextAccessor();
            #endregion

            #region Validation
            services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();

            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
            .AddMvc(x => x.Filters.Add(new ValidationFilter())).AddFluentValidation(config => { config.AutomaticValidationEnabled = true; });

            services.AddTransient<IValidator<LoginUserModelDTO>, LoginUserModelDTOValidator>();
            services.AddTransient<IValidator<RegisterUserModelDTO>, RegisterUserModelDTOValidator>();
            services.AddTransient<IValidator<UserValidationModelDTO>, UserValidationModelDTOValidator>();
            services.AddTransient<IValidator<AddRoleModelDTO>, AddRoleModelDTOValidator>();
            services.AddTransient<IValidator<UpdateRoleModelDTO>, UpdateRoleModelDTOValidator>();
            services.AddTransient<IValidator<MailModelDTO>, MailModelDTOValidator>();
            services.AddTransient<IValidator<AddUserModelDTO>, AddUserModelDTOValidator>();
            services.AddTransient<IValidator<UpdateUserModelDTO>, UpdateUserModelDTOValidator>();
            services.AddTransient<IValidator<ChangeUserPasswordModelDTO>, ChangeUserPasswordModelDTOValidator>();
            services.AddTransient<IValidator<ConfirmationEmailModelDTO>, ConfirmationEmailModelDTOValidator>();
            services.AddTransient<IValidator<ForgetPasswordModelDTO>, ForgetPasswordModelDTOValidator>();
            services.AddTransient<IValidator<ReConfirmationEmailModelDTO>, ReConfirmationEmailModelDTOValidator>();
            services.AddTransient<IValidator<ResetPasswordModelDTO>, ResetPasswordModelDTOValidator>();

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
