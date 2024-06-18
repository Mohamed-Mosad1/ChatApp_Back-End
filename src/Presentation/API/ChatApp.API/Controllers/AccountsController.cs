using ChatApp.Application.Features.Accounts.Command.CheckUserNameOrEmailExist;
using ChatApp.Application.Features.Accounts.Command.Login;
using ChatApp.Application.Features.Accounts.Command.Register;
using ChatApp.Application.Features.Accounts.Command.RemovePhoto;
using ChatApp.Application.Features.Accounts.Command.SetMainPhoto;
using ChatApp.Application.Features.Accounts.Command.UpdateCurrentMember;
using ChatApp.Application.Features.Accounts.Command.UploadPhoto;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Features.Accounts.Queries.GetCurrentUser;
using ChatApp.Application.Features.Accounts.Queries.GetUserByUserId;
using ChatApp.Application.Features.Accounts.Queries.GetUserByUserName;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.Extensions;
using ChatApp.Persistence.Helpers;
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

        /// <summary>
        /// Login with username and password to start chatting
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new LoginCommand(loginDto);
            var response = await _mediator.Send(command);

            return response.IsSuccess switch
            {
                true => Ok(response.Data),
                false when response.Message == "UnAuthorized" => Unauthorized(),
                false when response.Message == "NotFound" => NotFound(),
                _ => BadRequest(response.Message)
            };
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
        [AllowAnonymous]
        public async Task<ActionResult<RegisterDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new RegisterCommand(registerDto);
            var response = await _mediator.Send(command);

            return response.IsSuccess switch
            {
                true => Ok(response.Data),
                false when response.Errors != null => BadRequest(new { Errors = response.Errors }),
                _ => BadRequest("An unknown error occurred.")
            };
        }

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

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetAllUsers([FromQuery] UserParams userParams, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery(userParams), cancellationToken);
                if (users is not null)
                {
                    Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
                    return Ok(users);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Member")]
        [HttpGet("get-user-by-userName/{userName}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return NotFound();
                }

                var user = await _mediator.Send(new GetUserByUserNameQuery(userName), cancellationToken);
                return user is not null ? Ok(user) : NotFound();
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
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound();
                }

                var user = await _mediator.Send(new GetUserByUserIdQuery(userId), cancellationToken);
                return user is not null ? Ok(user) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-current-member")]
        public async Task<ActionResult<UpdateCurrentMemberDto>> UpdateCurrentMember([FromBody] UpdateCurrentMemberDto updateCurrentMemberDto)
        {
            try
            {
                var command = new UpdateCurrentMemberCommand(updateCurrentMemberDto);
                var response = await _mediator.Send(command);
                if (response.IsSuccess) 
                    return Ok(response.Data);

                return BadRequest(response.Errors);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This Endpoint Take file(image) and added photo to table
        /// api/Accounts/upload-photo
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// Object Of Photo Class
        /// </returns>
        /// <remarks>
        /// Take From File
        /// </remarks>
        [HttpPost("upload-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto(IFormFile file)
        {
            try
            {
                var command = new UploadPhotoCommand() { PhotoFile = file };
                var response = await _mediator.Send(command);
                if (response is not null)
                    return Ok("Photo Uploaded Successfully");

                return BadRequest("Unable to upload photo");
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to upload photo {ex.Message}");
            }
        }

        /// <summary>
        /// This Endpoint Remove Member Photo
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns></returns>
        /// <remarks>
        /// api/Accounts/remove-photo/{photoId}
        /// </remarks>
        [HttpDelete("remove-photo/{photoId}")]
        public async Task<IActionResult> RemovePhoto(int photoId)
        {
            try
            {
                var command = new RemovePhotoCommand(photoId);
                var response = await _mediator.Send(command);
                if (response)
                    return Ok("Photo Removed Successfully");

                return BadRequest("Unable to Remove photo");
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to Remove photo {ex.Message}");
            }
        }

        /// <summary>
        /// This Endpoint Set Main Photo
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns></returns>
        /// <remarks>
        /// URL => api/Accounts/set-main-photo/{photoId}
        /// </remarks>
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            try
            {
                if(photoId > 0)
                {
                    var command = new SetMainPhotoCommand(photoId);
                    var response = await _mediator.Send(command);
                    if (response)
                        return Ok("Photo Assigned Successfully");

                }

                return NotFound($"This ID {photoId} is not found");
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to set main photo {ex.Message}");
            }
        }

    }
}
