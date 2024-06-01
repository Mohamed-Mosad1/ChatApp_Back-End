using ChatApp.Application.Features.Accounts.Command.GetCurrentUser;
using ChatApp.Application.Features.Accounts.Command.GetCurrentUser.CheckUserNameOrEmailExist;
using ChatApp.Application.Features.Accounts.Command.Login;
using ChatApp.Application.Features.Accounts.Command.Register;
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
                if(result)
                    return Ok(result);

                return NotFound(false);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
