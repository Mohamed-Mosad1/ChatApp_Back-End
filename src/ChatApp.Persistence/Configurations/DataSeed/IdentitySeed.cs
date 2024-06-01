using ChatApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Persistence.Configurations.DataSeed
{
    public class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create Role
            if (!roleManager.Roles.Any())
            {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole(){ Name = "Admin"},
                    new IdentityRole(){ Name = "Moderator"},
                    new IdentityRole(){ Name = "Member"}

                };
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }


            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    UserName = "Mohamed",
                    Email = "mohamed@gmail.com",
                    Gender = "Male",
                    City = "Alex",
                    Country = "Alexandria",
                    DateOfBirth = new DateTime(2000, 03, 29),
                    KnownAs = "Mohamed Mosaad",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "P@$$W0rd");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            
        }
    }
}
