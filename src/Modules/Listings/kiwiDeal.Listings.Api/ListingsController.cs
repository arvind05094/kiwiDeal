using Asp.Versioning;
using kiwiDeal.Listings.Api.Requests;
using kiwiDeal.Listings.Application.Commands;
using kiwiDeal.Listings.Application.Queries;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.Listings.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings")]
public sealed class ListingsController(ISender sender, IImageService imageService, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateListing(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateListingCommand(
            currentUser.Id!.Value,
            request.Title,
            request.Description,
            request.StartingPrice);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return CreatedAtAction(nameof(GetListing), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateListing(
        Guid id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateListingCommand(
            id,
            currentUser.Id!.Value,
            request.Title,
            request.Description,
            request.StartingPrice);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteListing(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteListingCommand(id, currentUser.Id!.Value);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyListingsQuery(currentUser.Id!.Value, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListing(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetListingQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetListingsQuery(pageNumber, pageSize, searchTerm);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/images")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest("No file provided.");

        using var stream = file.OpenReadStream();
        var imageUrl = await imageService.UploadImageAsync(
            id,
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        return Ok(new { imageUrl });
    }
}
