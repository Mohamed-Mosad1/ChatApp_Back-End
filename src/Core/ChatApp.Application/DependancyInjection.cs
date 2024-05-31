using ChatApp.Application.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ChatApp.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            // Configure AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Configure MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));



            return services;
        }
    }
}
