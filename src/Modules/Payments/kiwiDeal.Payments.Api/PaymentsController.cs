using Asp.Versioning;
using kiwiDeal.Payments.Api.Requests;
using kiwiDeal.Payments.Application.Commands.CreateCheckoutSession;
using kiwiDeal.Payments.Application.Commands.HandleWebhook;
using kiwiDeal.Payments.Application.Queries.GetPayment;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.Payments.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
public sealed class PaymentsController(ISender sender) : ControllerBase
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
        [FromBody] HandleWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new HandleWebhookCommand(request.StripeSessionId, request.EventType);
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
}
