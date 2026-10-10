using FluentValidation;

namespace SITRA.API.Filters
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
            {
                return await next(context);
            }

            var toValidate = context.Arguments.OfType<T>().FirstOrDefault();

            if (toValidate is null)
            {
                return TypedResults.Problem(SITRA.Core.Helpers.Utilities.NoEntityToValidateMessage);
            }

            var validationResult = await validator.ValidateAsync(toValidate);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
