using DemoWeb.Infrastructure;
using DemoWeb.Infrastructure.Constants;
using DemoWeb.Infrastructure.Databases.DemoWebDB;
using DemoWeb.Infrastructure.Databases.DemoWebDB.Entities;
using DemoWeb.Service.Interfaces;
using DemoWeb.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace DemoWeb.Service
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddServiceCollection(this IServiceCollection services)
        {
            #region Common services
            // JWT
            //services.AddScoped<IJwtService, JwtService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<ILogService, LogService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();

            // User Management Service.
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>, SignInManager<User>>();

            // Auto mapper
            services.AddAutoMapper(typeof(ServiceCollection).Assembly);

            // Add Mediator
            //services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            // Service
            ///services.AddTransient<IFtpDirectoryService, FtpDirectoryService>();
            #endregion

            #region Business services
            #endregion
            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Application Database context
            services.AddSingleton<IDbContextFactory, DbContextFactory>();

            services.AddDbContext<DemoWebDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString: configuration.GetConnectionString("DemoWeb"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.CommandTimeout((int)TimeSpan.FromSeconds(SettingConstants.DatabaseSettings.TIMEOUT_FROM_SECONDS).TotalSeconds);
                        //sqlOptions.EnableRetryOnFailure();
                    });
                options.EnableDetailedErrors();
            });

            services.AddIdentity<User, IdentityRole<int>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DemoWebDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // Adding JWT Bearer
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidAudience = configuration["JWT: ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                    };
                });

            return services;
        }
    }
}
