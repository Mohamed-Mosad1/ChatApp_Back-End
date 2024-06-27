using AutoMapper;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApp.Application.Features.Accounts.Command.UpdateCurrentMember
{
    public class UpdateCurrentMemberCommand : IRequest<BaseCommonResponse>
    {
        public UpdateCurrentMemberDto _updateCurrentMemberDto { get; set; }

        public UpdateCurrentMemberCommand(UpdateCurrentMemberDto updateCurrentMemberDto)
        {
            _updateCurrentMemberDto = updateCurrentMemberDto;
        }

        class Handler : IRequestHandler<UpdateCurrentMemberCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(UserManager<AppUser> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<BaseCommonResponse> Handle(UpdateCurrentMemberCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new BaseCommonResponse();
                try
                {
                    var userName = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value;
                    if (userName is not null)
                    {
                        var currentUser = await _userManager.FindByNameAsync(userName);
                        currentUser.Introduction = request._updateCurrentMemberDto.Introduction;
                        currentUser.Interests = request._updateCurrentMemberDto.Interests;
                        currentUser.LookingFor = request._updateCurrentMemberDto.LookingFor;
                        currentUser.City = request._updateCurrentMemberDto.City;
                        currentUser.Country = request._updateCurrentMemberDto.Country;
                        var res = await _userManager.UpdateAsync(currentUser);

                        if (res.Succeeded)
                        {
                            response.IsSuccess = true;
                            response.Message = "User Updated Successfully";
                            response.Data = request._updateCurrentMemberDto;
                            return response;
                        }
                        else
                        {
                            foreach (var err in res.Errors)
                            {
                                response.Errors.Add($"Code: {err.Code} - Description: {err.Description}");
                            }
                            response.Message = "Bad Request";
                            return response;
                        }
                    }

                    response.Message = "User Name Invalid";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    return response;
                }
            }
        }
    }
}
