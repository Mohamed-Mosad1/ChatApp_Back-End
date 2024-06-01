using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Query.GetAllMessages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<MessageReturnDto>> GetAllMessages(CancellationToken cancellationToken)
        {
            var messages = await _mediator.Send(new GetAllMessagesQuery(), cancellationToken);

            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<MessageReturnDto>> AddMessage([FromBody] AddMessageDto addMessageDto, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var command = new AddMessageCommand(addMessageDto);
                    var response = await _mediator.Send(command);

                    return Ok(response.IsSuccess ? Ok(response) : BadRequest(response.Message));
                }

                return BadRequest("Error while adding new message");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
