using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.SharedKernel.Behaviours;

public sealed class AuthenticationBehaviour<TRequest, TResponse>(ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IPublicRequest)
            return await next();

        if (!currentUser.IsAuthenticated)
        {
            var error = Error.Unauthorised("User is not authenticated.");
            object result = Result.Failure(error);
            return (TResponse)result;
        }

        return await next();
    }
}
