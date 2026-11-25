using Application.DTOs.Contracts;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class ConfigureService
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["JWTConfiguration:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("JWTConfiguration:Key is not configured.");
            }

            services.AddSingleton(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                )
            );
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddSingleton<IConnectionMultiplexer>(opt =>
            {
                var options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            //identity
            services.AddIdentityService(configuration);
            return services;
        }
    }
}