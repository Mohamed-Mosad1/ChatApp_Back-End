using ChatApp.Application.Features.Accounts.Command.Login;
using ChatApp.Application.Features.Accounts.Command.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

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

        [HttpPost("Register")]
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

    }
}
