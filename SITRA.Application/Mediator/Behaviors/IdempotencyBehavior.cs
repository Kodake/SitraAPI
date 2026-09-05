using Mediator;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using System.Text.Json;

namespace SITRA.Core.Mediator.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public IdempotencyBehavior(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
        {
            // Only apply to commands marked with IIdempotentCommand
            if (request is not IIdempotentCommand)
            {
                return await next(request, cancellationToken);
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return await next(request, cancellationToken);
            }

            // Check for Idempotency Key
            if (!httpContext.Request.Headers.TryGetValue("X-Idempotency-Key", out var idempotencyKey) || !Guid.TryParse(idempotencyKey, out var key))
            {
                throw new FluentValidation.ValidationException(Utilities.IdempotencyKeyRequiredMessage);
            }

            // Check if key exists
            var existingRequests = await _unitOfWork.Repository<IdempotentRequest>().GetAllAsync(r => r.Id == key);
            var existingRequest = existingRequests.FirstOrDefault();

            if (existingRequest != null)
            {
                // Return stored response
                // We need to deserialize the response body to TResponse.
                // Note: TResponse might be complex or just a primitive/Mediator Unit.
                if (typeof(TResponse) == typeof(Unit))
                {
                    return (TResponse)(object)Unit.Value;
                }

                if (!string.IsNullOrEmpty(existingRequest.ResponseBody))
                {
                    return JsonSerializer.Deserialize<TResponse>(existingRequest.ResponseBody)!;
                }
            }

            // Execute Operation
            var response = await next(request, cancellationToken);

            // Store Result
            var idempotentRequest = new IdempotentRequest
            {
                Id = key,
                Name = typeof(TRequest).Name,
                CreatedOn = DateTime.UtcNow,
                ResponseStatusCode = 200, // Or extract from response if possible
                ResponseBody = JsonSerializer.Serialize(response)
            };

            await _unitOfWork.Repository<IdempotentRequest>().CreateAsync(idempotentRequest);
            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
