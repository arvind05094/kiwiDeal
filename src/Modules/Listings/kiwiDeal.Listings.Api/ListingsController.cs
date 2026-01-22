using Asp.Versioning;
using kiwiDeal.Listings.Api.Requests;
using kiwiDeal.Listings.Application.Commands;
using kiwiDeal.Listings.Application.Queries;
using kiwiDeal.Listings.Domain.Entities;
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
public sealed class ListingsController(ISender sender, ICurrentUser currentUser) : ControllerBase
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
            request.ListingType,
            request.BuyNowPrice,
            request.Category,
            request.Region);

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
            request.Description);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelListing(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelListingCommand(id, currentUser.Id!.Value);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImages(
        Guid id,
        IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        if (files.Count == 0)
            return BadRequest("No files provided.");

        if (files.Count > 3)
            return BadRequest("A maximum of 3 images can be uploaded.");

        var imageUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0)
                return BadRequest("One or more files are empty.");

            using var stream = file.OpenReadStream();
            var command = new UploadListingImageCommand(
                id,
                currentUser.Id!.Value,
                stream,
                file.FileName,
                file.ContentType);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblemDetails();

            imageUrls.Add(result.Value);
        }

        return Ok(new { imageUrls });
    }

    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ListingStatus[]? statuses = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyListingsQuery(currentUser.Id!.Value, pageNumber, pageSize, statuses);
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
        [FromQuery] string? category = null,
        [FromQuery] string? region = null,
        [FromQuery] string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetListingsQuery(pageNumber, pageSize, searchTerm, category, region, sortBy);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/watchlist")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToWatchlist(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new AddToWatchlistCommand(currentUser.Id!.Value, id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpDelete("{id:guid}/watchlist")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWatchlist(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new RemoveFromWatchlistCommand(currentUser.Id!.Value, id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpGet("watchlist")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWatchlist(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetWatchlistQuery(currentUser.Id!.Value, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }
}
