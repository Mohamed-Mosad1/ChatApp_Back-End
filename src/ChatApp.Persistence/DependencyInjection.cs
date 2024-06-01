using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.Configurations.DataSeed;
using ChatApp.Persistence.DatabaseContext;
using ChatApp.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Configure
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IMessageRepository), typeof(MessageRepository));

            // Configure Token Service
            services.AddScoped(typeof(ITokenService), typeof(TokenService));

            // Configure Identity
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                        ValidIssuer = configuration["JWT:Issuer"]
                    };
                });

            return services;
        }

        public static async void ConfigureMiddleware(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManger = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                await IdentitySeed.SeedUserAsync(userManger, roleManger);
            }
        }

    }
}
