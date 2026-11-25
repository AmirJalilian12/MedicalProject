using Domain.Entities.IdentityEntities;
using Domain.Exceptions;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public static class IdentityServiceExtension
    {
        public static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>()
                .AddTokenProvider("MyApp", typeof(DataProtectorTokenProvider<User>))
                .AddRoles<Role>()
                .AddRoleManager<RoleManager<Role>>()
                .AddRoleValidator<RoleValidator<Role>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure(ConfigureOptionsIdentity());
            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = OptionsTokenValidationParameters(configuration);
                    options.Events = JwtOptionsEvents();
                });
        }
        private static TokenValidationParameters OptionsTokenValidationParameters(IConfiguration configuration)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTConfiguration:Key"] ?? string.Empty)),
                ValidateIssuer = true,
                ValidIssuer = configuration["JWTConfiguration:Issuer"],
                ValidateAudience = Convert.ToBoolean(configuration["JWTConfiguration:Audience"]),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
        }
        private static Action<IdentityOptions> ConfigureOptionsIdentity()
        {
            return options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            };
        }
        private static JwtBearerEvents JwtOptionsEvents()
        {
            return new JwtBearerEvents
            {
                OnAuthenticationFailed = c =>
                {
                    c.NoResult();
                    c.Response.StatusCode = 500;
                    c.Response.ContentType = "application/json";
                    return c.Response.WriteAsync("مشکلی در سمت سرور رخ داده است لطفا مجددا تلاش بفرمایید");
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new ApiToReturn(401, "شما اهراز هویت نشده اید"));
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = contex =>
                {
                    contex.Response.StatusCode = 403;
                    contex.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new ApiToReturn(403, "شما به این بخش دسترسی ندارید لطفا ابتدا وارد سایت شوید"));
                    return contex.Response.WriteAsync(result);
                }
            };
        }
    }
}
