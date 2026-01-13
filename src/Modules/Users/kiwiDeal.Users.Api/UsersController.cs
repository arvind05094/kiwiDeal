using Asp.Versioning;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Api.Requests;
using kiwiDeal.Users.Application.Commands.SubmitRating;
using kiwiDeal.Users.Application.Commands.UpdateProfile;
using kiwiDeal.Users.Application.Queries.GetMyProfile;
using kiwiDeal.Users.Application.Queries.GetUserProfile;
using kiwiDeal.Users.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.Users.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public sealed class UsersController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserProfile(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserProfileQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var query = new GetMyProfileQuery(currentUser.Id!.Value);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(
            currentUser.Id!.Value,
            request.FirstName,
            request.LastName,
            request.Region);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpPost("{id:guid}/rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitRating(
        Guid id,
        [FromBody] SubmitRatingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SubmitRatingCommand(
            currentUser.Id!.Value,
            id,
            request.Stars,
            request.Comment);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }
}
