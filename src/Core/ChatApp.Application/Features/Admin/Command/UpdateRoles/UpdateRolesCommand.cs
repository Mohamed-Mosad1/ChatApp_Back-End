using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Admin.Command.UpdateRoles
{
    public class UpdateRolesCommand : IRequest<BaseCommonResponse>
    {
        public string UserName { get; set; }
        public string Roles { get; set; }

        public UpdateRolesCommand(string userName, string roles)
        {
            UserName = userName;
            Roles = roles;
        }

        class Handler : IRequestHandler<UpdateRolesCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;

            public Handler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<BaseCommonResponse> Handle(UpdateRolesCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();

                try
                {
                    // Admin,Member,...
                    var selectedRoles = request.Roles.Split(',').ToArray();

                    var user = await _userManager.FindByNameAsync(request.UserName);
                    if (user is null)
                    {
                        response.Message = "User not found";
                        response.Errors.Add("User not found");

                        return response;
                    }

                    var userRoles = await _userManager.GetRolesAsync(user);
                    var rolesToAdd = selectedRoles.Except(userRoles);
                    var rolesToRemove = userRoles.Except(selectedRoles);

                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        response.Message = "Error while adding roles";
                        response.Errors.AddRange(addResult.Errors.Select(e => e.Description));

                        return response;
                    }

                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        response.Message = "Error while removing roles";
                        response.Errors.AddRange(removeResult.Errors.Select(e => e.Description));

                        return response;
                    }

                    response.IsSuccess = true;
                    response.Message = "Roles updated successfully";
                    response.Data = await _userManager.GetRolesAsync(user);

                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = "An error occurred while updating roles";
                    response.Errors.Add(ex.Message);

                    return response;
                }
            }
        }
    }
}
