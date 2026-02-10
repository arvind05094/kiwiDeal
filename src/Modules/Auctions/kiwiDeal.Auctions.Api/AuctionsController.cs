using Asp.Versioning;
using kiwiDeal.Auctions.Api.Requests;
using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Auctions.Application.Queries;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kiwiDeal.Listings.Application.Commands;
using kiwiDeal.Listings.Application.Queries;

namespace kiwiDeal.Auctions.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auctions")]
public sealed class AuctionsController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAuction(
        [FromBody] CreateAuctionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAuctionCommand(
            request.ListingId,
            request.ListingTitle,
            currentUser.Id!.Value,
            request.StartingPrice,
            request.StartTime,
            request.EndTime);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return CreatedAtAction(nameof(GetAuction), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id:guid}/bids")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PlaceBid(
        Guid id,
        [FromBody] PlaceBidRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PlaceBidCommand(id, currentUser.Id!.Value, currentUser.Name ?? "Unknown", request.Amount);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAuction(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CloseAuctionCommand(id, currentUser.Id!.Value);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuction(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetAuctionQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuctions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool endingSoon = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuctionsQuery(pageNumber, pageSize, endingSoon);
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
        var auctionQuery = new GetAuctionQuery(id);
        var auctionResult = await sender.Send(auctionQuery, cancellationToken);

        if (auctionResult.IsFailure)
            return auctionResult.Error.ToProblemDetails();

        var command = new AddToAuctionWatchlistCommand(
            currentUser.Id!.Value,
            id,
            auctionResult.Value.SellerId,
            auctionResult.Value.Status);

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
        var command = new RemoveFromAuctionWatchlistCommand(currentUser.Id!.Value, id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return NoContent();
    }

    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAuctions(
    [FromQuery] string? status,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query = new GetMyAuctionsQuery(currentUser.Id!.Value, status, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet("bidding")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBiddingAuctions(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBiddingAuctionsQuery(currentUser.Id!.Value, status, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }

    [HttpGet("watchlist")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuctionWatchlist(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query = new GetAuctionWatchlistQuery(currentUser.Id!.Value, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblemDetails();

        return Ok(result.Value);
    }
}
