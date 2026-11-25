using Application.Common.BehaviorPipes;
using Application.Common.Mapping;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ConfigureService
    {
        public static IServiceCollection AddApplicationService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            // ۱) مپ‌های وابسته به IConfiguration باید دستی ثبت شوند
            new ProductMappingConfig(
                configuration["BackendUrl"],
                configuration["LocationImages:ProductImages"]
            ).Register(config);

            // ۲) بقیه‌ی IRegisterها (مثل MappingProfile) اسکن می‌شوند
            config.Scan(Assembly.GetExecutingAssembly());
            // ۳) Mapster
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            // ۴) MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
