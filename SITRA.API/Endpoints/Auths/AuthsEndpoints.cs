using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.API.Filters;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Queries.Auths;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SITRA.API.Endpoints
{
    public static class AuthsEndpoints
    {
        public static RouteGroupBuilder MapAuths(this RouteGroupBuilder group)
        {
            group.MapPost("/register", Register).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateRegistrationUserDTO>>()
                .WithOpenApi();
            group.MapPost("/login", Login).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateLoginUserDTO>>()
                .WithOpenApi();
            group.MapGet("/validate-opened-petty-cash", ValidateOpenedPettyCashEndpoint)
                .WithOpenApi().RequireAuthorization();

            return group;
        }

        static async Task<Results<Created, BadRequest<IEnumerable<IdentityError>>, ValidationProblem>> Register(
            [FromBody] CreateRegistrationUserDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new RegisterUserCommand(dto);
            var result = await mediator.Send(command);

            if (!result.Succeeded)
                return TypedResults.BadRequest(result.Errors);

            return TypedResults.Created();
        }

        static async Task<Results<Ok<TokenResponseDTO>, BadRequest, ValidationProblem>> Login(
            [FromBody] CreateLoginUserDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new LoginUserCommand(dto);
            var response = await mediator.Send(command);

            if (response == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok, BadRequest<string>>> ValidateOpenedPettyCashEndpoint(
            [FromServices] IMediator mediator)
        {
            var query = new ValidateOpenedPettyCashQuery();
            var (success, errorMessage) = await mediator.Send(query);

            if (!success)
            {
                return TypedResults.BadRequest(errorMessage ?? "Validación de caja chica fallida.");
            }

            return TypedResults.Ok();
        }
    }
}

