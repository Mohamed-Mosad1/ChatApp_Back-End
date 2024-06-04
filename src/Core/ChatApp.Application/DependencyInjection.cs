using ChatApp.Application.MappingProfiles;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            // Configure AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Configure MediatR
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());



            return services;
        }
    }
}
