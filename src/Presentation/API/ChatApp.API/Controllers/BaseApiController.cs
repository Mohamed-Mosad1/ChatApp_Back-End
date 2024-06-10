using ChatApp.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
