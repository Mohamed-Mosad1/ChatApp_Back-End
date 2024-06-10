using AutoMapper;
using ChatApp.Application.Features.Accounts.Validators;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
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
                BaseCommonResponse res = new BaseCommonResponse();

                try
                {
                    var validator = new RegisterValidator();
                    var validatorResult = await validator.ValidateAsync(request.RegisterDto, cancellationToken);
                    if (!validatorResult.IsValid)
                    {
                        res.IsSuccess = false;
                        res.Message = "While Validate Register Date";
                        res.Errors = validatorResult.Errors.Select(x=>x.ErrorMessage).ToList();
                        return res;
                    }

                    var user = _mapper.Map<RegisterDto, AppUser>(request.RegisterDto);

                    var response = await _userManager.CreateAsync(user, request.RegisterDto.Password);
                    if (!response.Succeeded)
                    {
                        foreach (var err in response.Errors)
                           res.Errors.Add($"{err.Code} - {err.Description}");
                        
                        res.IsSuccess = false;
                        res.Message = "BadRequest";
                        return res;
                    }

                    res.IsSuccess = true;
                    res.Message = "Register Success";
                    res.Data = new
                    {
                        userName = user.UserName,
                        knownAs = user.KnownAs,
                        email = user.Email,
                        gender = user.Gender,
                        token = await _tokenService.CreateTokenAsync(user),
                        photoUrl = user.Photos.FirstOrDefault(p=>p.IsMain)?.Url
                    };

                    return res;
                }
                catch (Exception ex)
                {
                    res.IsSuccess = false;
                    res.Message = ex.Message;

                    return res;
                }
            }
        }

    }
}
