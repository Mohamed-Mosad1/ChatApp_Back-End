﻿using ChatApp.Application.Features.Likes.Command.AddLike;
using ChatApp.Application.Features.Likes.Query.GetLikedUsers;
using ChatApp.Application.Helpers;
using ChatApp.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IMediator _mediator;

        public LikesController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add-like/{userName}")]
        public async Task<ActionResult> AddLike(string userName, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    var command = new AddLikeCommand(userName);
                    var response = await _mediator.Send(command);
                    if (response.IsSuccess)
                    {
                        return Ok(response);
                    }

                    return BadRequest(response.Message);
                }
                return NotFound(value: "User Name Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-liked-users")]
        public async Task<ActionResult<IReadOnlyList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams, CancellationToken cancellationToken)
        {
            try
            {
                var likes = await _mediator.Send(new GetUserLikesQuery(likesParams), cancellationToken);

                if (likes is not null)
                {
                    Response.AddPaginationHeader(likes.CurrentPage, likes.PageSize, likes.TotalCount, likes.TotalPages);
                    return Ok(likes);
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