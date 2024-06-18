using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Command.DeleteMessage;
using ChatApp.Application.Features.Messages.Command.GetMessagesIsRead;
using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Helpers;
using ChatApp.Persistence.Extensions;
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

        [HttpGet("get-messages-for-user")]
        public async Task<ActionResult<MessageDto>> GetMessageForUser([FromQuery] MessageParams messageParams, CancellationToken cancellationToken)
        {
            var messages = await _mediator.Send(new GetMessageForUserQuery(messageParams), cancellationToken);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            if (messages is not null)
                return Ok(messages);

            return NotFound();
        }

        [HttpPost("add-message")]
        public async Task<ActionResult<MessageDto>> AddMessage([FromBody] AddMessageDto addMessageDto, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var command = new AddMessageCommand(addMessageDto);
                    var response = await _mediator.Send(command, cancellationToken);

                    return response.IsSuccess ? Ok(response.Data) : BadRequest(response.Message);
                }

                return BadRequest("Error while adding new message");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mark-message-as-read/{userName}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessagesIsRead(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new GetMessagesIsReadCommand(userName), cancellationToken);

                if (response is not null)
                    return Ok(response);

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-message/{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            try
            {
                var command = new DeleteMessageCommand(id);
                var response = await _mediator.Send(command, CancellationToken.None);
                if (response.IsSuccess == false && response.Message.Contains("Unauthorized"))
                    return Unauthorized();

                if (response.IsSuccess)
                    return Ok(response);

                return NotFound($"Message with ID {id} not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
