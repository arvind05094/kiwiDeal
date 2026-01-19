using Asp.Versioning;
using kiwiDeal.Messages.Application.Commands;
using kiwiDeal.Messages.Application.Queries;
using kiwiDeal.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Messages.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/messages")]
[Authorize]
public sealed class MessagesController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation(
        [FromBody] Requests.StartConversationRequest request,
        CancellationToken ct)
    {
        var command = new StartConversationCommand(
            request.ListingId,
            currentUser.Id!.Value,
            request.RecipientId,
            request.InitialMessage);

        var result = await sender.Send(command, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid conversationId,
        [FromBody] Requests.SendMessageRequest request,
        CancellationToken ct)
    {
        var command = new SendMessageCommand(
            conversationId,
            currentUser.Id!.Value,
            request.Content);

        var result = await sender.Send(command, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(CancellationToken ct)
    {
        var query = new GetConversationsQuery(currentUser.Id!.Value);
        var result = await sender.Send(query, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }

    [HttpGet("conversations/{conversationId:guid}")]
    public async Task<IActionResult> GetMessages(Guid conversationId, CancellationToken ct)
    {
        var query = new GetMessagesQuery(conversationId, currentUser.Id!.Value);
        var result = await sender.Send(query, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }
}