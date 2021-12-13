using dms_backend_api.Data;
using dms_backend_api.Domain.Identity;
using dms_backend_api.Helpers;
using dms_backend_api.Infrastracture;
using dms_backend_api.Infrastracture.Jobs.Internall;
using dms_backend_api.Infrastracture.Middlewares;
using dms_backend_api.Services.Identity;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace dms_backend_api
{
    public partial class Startup
    {
        #region Fields

        const string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private static IConfiguration? _configuration;
        public Domain.Postgres.DatabaseConnection ConnProp { get; set; }

        #endregion

        #region Ctor

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnProp = PostgresHelper.TransferPostgreUrlToConnection((string)_configuration.GetValue(typeof(string), "postgresqlURL"));
        }

        #endregion

        public void ConfigureServices(IServiceCollection services)
        {
            #region DI
#pragma warning disable CS8604 // Possible null reference argument.
            new DependencyRegistrar().Register(services, _configuration);
#pragma warning restore CS8604 // Possible null reference argument.
            #endregion

            #region Identity
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql($"User ID={ConnProp.User};Password={ConnProp.Password};Host={ConnProp.Hostname};Port={ConnProp.Port};Database={ConnProp.DatabaseName}"); });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddTokenProvider("Default", typeof(UserTokenProvider<ApplicationUser>));
            #endregion

            #region SwaggerDocumentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "dms_backend_api", Version = "v1", Description = (bool)_configuration.GetValue(typeof(bool), "Hangfire:Hangfire_Active", false) ? $"<a href=\"/jobs\">Hangfire jobs</a>" : "" });
                c.EnableAnnotations();
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer xxxxxxxx'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                });
            });
            #endregion

            #region JWTAuthorization
            var JWTSecret = (string)_configuration.GetValue(typeof(string), "JWTSecret");
            if (string.IsNullOrEmpty(JWTSecret))
                throw new InvalidOperationException("The JWT Secret is empty.");

            services.AddCors(options =>
                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                )
            );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = (string)_configuration.GetValue(typeof(string), "JWT_ValidAudience"),
                    ValidIssuer = (string)_configuration.GetValue(typeof(string), "JWT_ValidIssuer"),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((string)_configuration.GetValue(typeof(string), "JWTSecret")))
                };
            });
            #endregion

            #region Hangfire
            if ((bool)_configuration.GetValue(typeof(bool), "Hangfire:Hangfire_Active", false))
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = (int)_configuration.GetValue(typeof(int), "Hangfire:Worker_Count", 1);
                });

                var sbSql = new NpgsqlConnectionStringBuilder($"User ID={ConnProp.User};Password={ConnProp.Password};Host={ConnProp.Hostname};Port={ConnProp.Port};Database={ConnProp.DatabaseName}")
                {
                    Pooling = (bool)_configuration.GetValue(typeof(bool), "Hangfire:Pooling", false)
                };

                services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer().UseDefaultTypeSerializer().UseSerilogLogProvider().UsePostgreSqlStorage(sbSql.ToString(),
                new PostgreSqlStorageOptions() { SchemaName = (string)_configuration.GetValue(typeof(string), "DB_SCHEMA"), UseNativeDatabaseTransactions = true }));
            }
            #endregion

            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
              connectionString: $"User ID={ConnProp.User};Password={ConnProp.Password};Host={ConnProp.Hostname};Port={ConnProp.Port};Database={ConnProp.DatabaseName}",
              tableName: "backend-log",
              restrictedToMinimumLevel: LogEventLevel.Information,
              needAutoCreateTable: true,
              respectCase: true,
              useCopy: false
        ).CreateLogger();

            Log.Logger.Information("Application Starting");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dms_backend_api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_myAllowSpecificOrigins);

            app.UseJWTInHeaderMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Hangfire
            if ((bool)_configuration.GetValue(typeof(bool), "Hangfire:Hangfire_Active", false))
            {
                var recuringStaticJobs = serviceProvider.GetService<IRecurringStaticJobs>();
                if (recuringStaticJobs is not null)
                    recuringStaticJobs.AddStaticRecuringJobs();

                app.UseHangfireDashboard("/jobs", new DashboardOptions() { Authorization = new[] { new HangfireAuthorizationFilter() }, AppPath = "/swagger/index.html" });
            }
            #endregion
        }
    }
}
