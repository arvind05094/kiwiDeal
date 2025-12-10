using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Commands.HandleWebhook;

public sealed record HandleWebhookCommand(
    string StripeSessionId,
    string EventType) : IRequest<Result>, IPublicRequest;
