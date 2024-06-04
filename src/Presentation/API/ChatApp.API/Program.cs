using ChatApp.Application;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence;
using ChatApp.Persistence.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ChatApp.API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(s =>
            {
                var securitySchema = new OpenApiSecurityScheme()
                {
                    Name = "Authorizathion",
                    Description = "JWT Auth Bearer",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference()
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                s.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement() { { securitySchema, new[] { "Bearer" } } };
                s.AddSecurityRequirement(securityRequirement);

                // Configure XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
            });

            // Configure External Project(dll)
            builder.Services.ConfigureApplicationServices();
            builder.Services.ConfigurePersistenceServices(builder.Configuration);

            // Enable Cors
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            #region Apple All Pending Migrations and Data Seeding

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var _dbContext = services.GetRequiredService<ApplicationDbContext>();
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();

                try
                {
                    await _dbContext.Database.MigrateAsync(); // Update StoreContext Database
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while updating the database.");
                }

            }

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.ConfigureMiddleware();

            app.Run();
        }
    }
}
