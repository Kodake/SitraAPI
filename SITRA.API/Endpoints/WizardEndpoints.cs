using SITRA.Core.DTOs.Wizard;
using SITRA.API.Filters;
using SITRA.Core.Mediator.Commands.Wizard;
using SITRA.Core.Mediator.Queries.Wizard;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SITRA.API.Endpoints
{
    public static class WizardEndpoints
    {
        public static RouteGroupBuilder MapWizards(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<WizardDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapGet("/is-required", IsRequired).RequireAuthorization();
            return group;
        }

        static async Task<Results<Ok<WizardDTO>, ValidationProblem>> Create(
            [FromBody] WizardDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateWizardCommand(dto);
            var result = await mediator.Send(command);
            return TypedResults.Ok(result);
        }

        static async Task<Ok<bool>> IsRequired(
            [FromServices] IMediator mediator)
        {
            var query = new IsWizardRequiredQuery();
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }
    }
}
