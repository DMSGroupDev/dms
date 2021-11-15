using dms_backend_api.Data;
using dms_backend_api.Domain.Identity;
using dms_backend_api.Helpers;
using dms_backend_api.Infrastracture;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace dms_backend_api
{
    public partial class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static IConfiguration? _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = Configuration = configuration;
            ConnProp = PostgresHelper.TransferPostgreUrlToConnection((string)Configuration.GetValue(typeof(string), "postgresqlURL"));
        }

        public Domain.Postgres.DatabaseConnection ConnProp { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            new DependencyRegistrar().Register(services, _configuration);
#pragma warning restore CS8604 // Possible null reference argument.

            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql($"User ID={ConnProp.User};Password={ConnProp.Password};Host={ConnProp.Hostname};Port={ConnProp.Port};Database={ConnProp.DatabaseName}"); });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddTokenProvider("Default", typeof(UserTokenProvider<ApplicationUser>));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "dms_backend_api", Version = "v1" });
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

            var JWTSecret = (string)_configuration.GetValue(typeof(string), "JWTSecret");
            if (string.IsNullOrEmpty(JWTSecret))
                throw new InvalidOperationException("The JWT Secret is empty.");

            services.AddCors(options =>
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                )
            );

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((string)_configuration.GetValue(typeof(string), "JWTSecret")))
                };
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                .ReadFrom.Configuration(Configuration) // connect serilog to our configuration folder
                .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
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

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
