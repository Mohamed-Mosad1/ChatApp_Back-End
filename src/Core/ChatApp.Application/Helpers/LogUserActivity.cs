using ChatApp.Application.Persistence.Contracts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ChatApp.Application.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Proceed with the action execution
            var resultContext = await next();

            // Check if the user is authenticated
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            // Get the username from claims
            var userName = resultContext.HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value;

            if (string.IsNullOrEmpty(userName))
                return;

            // Resolve the IUserRepository service
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            if (repo == null)
                return;

            try
            {
                // Get the user by username
                var user = await repo.GetUserByUserNameAsync(userName);

                if (user == null)
                    return;

                // Update the last active time
                user.LastActive = DateTime.Now;

                // Save the changes
                await repo.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"An error occurred while logging user activity: {ex.Message}");
            }
        }
    }
}
