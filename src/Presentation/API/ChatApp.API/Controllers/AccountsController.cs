using ChatApp.Application.Features.Accounts.Command.CheckUserNameOrEmailExist;
using ChatApp.Application.Features.Accounts.Command.Login;
using ChatApp.Application.Features.Accounts.Command.Register;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Features.Accounts.Queries.GetCurrentUser;
using ChatApp.Application.Features.Accounts.Queries.GetUserByUserId;
using ChatApp.Application.Features.Accounts.Queries.GetUserByUserName;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var command = new LoginCommand(loginDto);
                    var response = await _mediator.Send(command);
                    if (response.IsSuccess)
                        return Ok(response.Data);

                    if (response.IsSuccess == false && response.Message == "UnAuthorized")
                        return Unauthorized();

                    if (response.IsSuccess == false && response.Message == "NotFound")
                        return NotFound();

                    return BadRequest(response.Message);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Take Data From Body
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns>
        /// Return Token-UserName-Email
        /// </returns>
        /// <remarks>
        /// Roles:1=Admin,2=Member]
        /// //BaseUrl + /api/Register
        /// </remarks>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RegisterDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var command = new RegisterCommand(registerDto);
                    var response = await _mediator.Send(command);
                    if (response.IsSuccess)
                        return Ok(response.Data);

                    if (response.IsSuccess == false)
                        return BadRequest(response.Errors);

                    return BadRequest(response.Message);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get-current-user")]
        public async Task<ActionResult<UserToReturnDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
                if (user is not null)
                    return Ok(user);

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("check-userName-or-email-exist/{searchTerm}")]
        public async Task<ActionResult<bool>> CheckUserNameOrEmailExist(string searchTerm, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new CheckUserNameOrEmailExistQuery(searchTerm), cancellationToken);
                if (result)
                    return Ok(result);

                return NotFound(false);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all-user")]
        public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
                if (users is not null)
                    return Ok(users);

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-user-by-userName/{userName}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = await _mediator.Send(new GetUserByUserNameQuery(userName), cancellationToken);
                    if (user is not null)
                        return Ok(user);

                    return NotFound();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-user-by-Id/{userId}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserId(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _mediator.Send(new GetUserByUserIdQuery(userId), cancellationToken);
                    if (user is not null)
                        return Ok(user);

                    return NotFound();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
