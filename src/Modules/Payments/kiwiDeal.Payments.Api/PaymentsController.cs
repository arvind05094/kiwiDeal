using Asp.Versioning;
using kiwiDeal.Payments.Api.Requests;
using kiwiDeal.Payments.Application;
using kiwiDeal.Payments.Application.Commands.CreateBuyNowCheckoutSession;
using kiwiDeal.Payments.Application.Commands.CreateCheckoutSession;
using kiwiDeal.Payments.Application.Commands.HandleWebhook;
using kiwiDeal.Payments.Application.Queries.GetPayment;
using kiwiDeal.Payments.Application.Queries.GetPaymentByListing;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.Payments.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
public sealed class PaymentsController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCheckoutSession(
        [FromBody] CreateCheckoutSessionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCheckoutSessionCommand(request.AuctionId);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToProblemDetails();
        return Ok(new { checkoutUrl = result.Value });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleWebhook(
        [FromServices] IStripeWebhookVerifier verifier,
        CancellationToken cancellationToken)
    {
        HttpContext.Request.EnableBuffering();
        HttpContext.Request.Body.Position = 0;

        string payload;
        using (var reader = new StreamReader(HttpContext.Request.Body, leaveOpen: true))
            payload = await reader.ReadToEndAsync(cancellationToken);

        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

        var verifyResult = verifier.VerifyAndParse(payload, stripeSignature);
        if (verifyResult.IsFailure)
            return BadRequest(new { error = verifyResult.Error.Message });

        var command = new HandleWebhookCommand(verifyResult.Value.SessionId, verifyResult.Value.EventType);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToProblemDetails();
        return NoContent();
    }

    [HttpGet("{auctionId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(
        Guid auctionId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentQuery(auctionId);
        var result = await sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }

    [HttpPost("buynow")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBuyNowCheckoutSession(
    [FromBody] CreateBuyNowCheckoutSessionRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreateBuyNowCheckoutSessionCommand(
            request.ListingId,
            currentUser.Id!.Value,
            request.SellerId,
            request.Amount);
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToProblemDetails();
        return Ok(new { checkoutUrl = result.Value });
    }

    [HttpGet("listing/{listingId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentByListing(
    Guid listingId,
    CancellationToken cancellationToken)
    {
        var query = new GetPaymentByListingQuery(listingId);
        var result = await sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }
}
