using AutoMapper;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using MediatR;

namespace ChatApp.Application.Features.Accounts.Queries.GetUserByUserId
{
    public class GetUserByUserIdQuery : IRequest<MemberDto>
    {
        public string UserId { get; set; }

        public GetUserByUserIdQuery(string userId)
        {
            UserId = userId;
        }
        class Handler : IRequestHandler<GetUserByUserIdQuery, MemberDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }

            public async Task<MemberDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!string.IsNullOrEmpty(request.UserId))
                    {
                        var user = await _userRepository.GetUserByIdAsync(request.UserId);
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
