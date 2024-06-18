using AutoMapper;
using ChatApp.Application.Features.Accounts.Validators;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Accounts.Command.Register
{
    public class RegisterCommand : IRequest<BaseCommonResponse>
    {
        private readonly RegisterDto RegisterDto;

        public RegisterCommand(RegisterDto registerDto)
        {
            RegisterDto = registerDto;
        }

        class Handler : IRequestHandler<RegisterCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenService _tokenService;
            private readonly IMapper _mapper;

            public Handler(
                UserManager<AppUser> userManager,
                ITokenService tokenService,
                IMapper mapper
                )
            {
                _userManager = userManager;
                _tokenService = tokenService;
                _mapper = mapper;
            }

            public async Task<BaseCommonResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();

                var validator = new RegisterValidator();
                var validationResult = await validator.ValidateAsync(request.RegisterDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = "Validation Failed";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                var user = _mapper.Map<RegisterDto, AppUser>(request.RegisterDto);

                var identityResult = await _userManager.CreateAsync(user, request.RegisterDto.Password);
                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!identityResult.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = "User creation failed";
                    response.Errors = identityResult.Errors.Select(e => $"{e.Code} - {e.Description}").ToList();
                    return response;
                }
                
                if (!roleResult.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = "User role failed to add";
                    response.Errors = roleResult.Errors.Select(e => $"{e.Code} - {e.Description}").ToList();
                    return response;
                }

                var token = await _tokenService.CreateTokenAsync(user);
                response.IsSuccess = true;
                response.Message = "Registration successful";
                response.Data = new
                {
                    userName = user.UserName,
                    knownAs = user.KnownAs,
                    email = user.Email,
                    gender = user.Gender,
                    token,
                    photoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
                };

                return response;
            }
        }

    }
}
