using ChatApp.Application.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            // Configure AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Configure MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));



            return services;
        }
    }
}
