using ChatApp.Application.Features.Admin.Command.UpdateRoles;
using ChatApp.Application.Features.Admin.Queries.GetUsersWithRoles;
using ChatApp.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Only Admin can modify or view all users with roles
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-users-with-roles")]
        [Cached(600)]
        public async Task<ActionResult<IReadOnlyList<UsersWithRolesDto>>> GetUsersWithRoles()
        {
            var query = new GetUsersWithRolesQuery();
            var response = await _mediator.Send(query);

            if (response is not null)
                return Ok(response);

            return NotFound();
        }

        [HttpPost("update-roles/{userName}")]
        public async Task<ActionResult> UpdateRoles(string userName, [FromQuery] string roles)
        {
            var command = new UpdateRolesCommand(userName, roles);
            var response = await _mediator.Send(command, CancellationToken.None);
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response.Message);
        }




    }
}
