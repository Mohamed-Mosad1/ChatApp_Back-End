using AutoMapper;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using MediatR;

namespace ChatApp.Application.Features.Accounts.Queries.GetUserByUserName
{
    public class GetUserByUserNameQuery : IRequest<MemberDto>
    {
        public string UserName { get; set; }

        public GetUserByUserNameQuery(string userName)
        {
            UserName = userName;
        }

        class Handler : IRequestHandler<GetUserByUserNameQuery, MemberDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }
            public async Task<MemberDto> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!string.IsNullOrEmpty(request.UserName))
                    {
                        var user = await _userRepository.GetUserByUserNameAsync(request.UserName);
                        if (user is not null)
                        {
                            var result = _mapper.Map<AppUser, MemberDto>(user);

                            return result;
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }
    }
}
