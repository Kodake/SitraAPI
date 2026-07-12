using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Attributes;
using SITRA.API.Extensions;
using SITRA.API.Filters;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Queries.Auths;
using System;

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
            group.MapPost("/refresh", Refresh).DisableAntiforgery()
                .WithOpenApi();
            group.MapGet("/validate-opened-petty-cash", ValidateOpenedPettyCashEndpoint)
                .WithOpenApi().RequireAuthorization();
            group.MapGet("/validate-uploaded-certificate", ValidateUploadedCertificateRncEndpoint)
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/change-language", ChangeLanguage)
                .AddEndpointFilter<ValidationFilter<ChangeLanguageDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapGet("/me", GetUser)
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/update-profile", UpdateProfile)
                .AddEndpointFilter<ValidationFilter<UpdateProfileNameDTO>>()
                .DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();

            group.MapPost("/forgot-password", ForgotPassword)
                .DisableAntiforgery()
                .WithOpenApi().AllowAnonymous();
            group.MapPost("/reset-password", ResetPassword)
                .DisableAntiforgery()
                .WithOpenApi().AllowAnonymous();

            return group;
        }

        [Idempotent]
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

        static async Task<Results<Ok<TokenResponseDTO>, ProblemHttpResult, ValidationProblem>> Login(
            [FromBody] CreateLoginUserDTO dto,
            HttpContext httpContext,
            [FromServices] IMediator mediator)
        {
            var command = new LoginUserCommand(dto);
            var response = await mediator.Send(command);

            if (response == null)
            {
                return TypedResults.Problem(
                    detail: SITRA.Core.Helpers.Utilities.InvalidCredentialsMessage,
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }

            if (!string.IsNullOrEmpty(response.RefreshToken))
            {
                httpContext.AppendRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiration);
            }

            response.RefreshToken = null;

            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok<TokenResponseDTO>, UnauthorizedHttpResult>> Refresh(
            HttpContext httpContext,
            [FromServices] IMediator mediator)
        {
            var (expiredJwt, refreshToken) = httpContext.GetRefreshTokens();
            if (string.IsNullOrEmpty(expiredJwt) || string.IsNullOrEmpty(refreshToken))
            {
                return TypedResults.Unauthorized();
            }

            var command = new RefreshTokenCommand(expiredJwt, refreshToken);
            var response = await mediator.Send(command);

            if (response == null)
            {
                return TypedResults.Unauthorized();
            }

            if (!string.IsNullOrEmpty(response.RefreshToken))
            {
                httpContext.AppendRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiration);
            }

            response.RefreshToken = null;

            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok, BadRequest>> ForgotPassword(
            [FromBody] ForgotPasswordDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new ForgotPasswordCommand(dto);
            await mediator.Send(command);
            
            // Always return OK to prevent email enumeration attacks
            return TypedResults.Ok();
        }

        static async Task<Results<Ok, BadRequest>> ResetPassword(
            [FromBody] ResetPasswordDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new ResetPasswordCommand(dto);
            var success = await mediator.Send(command);
            
            if (!success)
                return TypedResults.BadRequest();

            return TypedResults.Ok();
        }

        static async Task<IResult> ValidateOpenedPettyCashEndpoint(
            [FromServices] IMediator mediator)
        {
            var query = new ValidateOpenedPettyCashQuery();
            var (success, errorMessage) = await mediator.Send(query);

            if (!success)
            {
                return TypedResults.BadRequest(new { success = false, message = errorMessage ?? SITRA.Core.Helpers.Utilities.PettyCashValidationFailedMessage });
            }

            return TypedResults.Ok(new { success = true, message = "Success" });
        }

        static async Task<IResult> ValidateUploadedCertificateRncEndpoint(
            [FromServices] IMediator mediator)
        {
            var query = new ValidateUploadedCertificateRncQuery();
            var (success, errorMessage) = await mediator.Send(query);

            if (!success)
            {
                return TypedResults.BadRequest(new { success = false, message = errorMessage ?? SITRA.Core.Helpers.Utilities.CertificateValidationFailedMessage });
            }

            return TypedResults.Ok(new { success = true, message = "Success" });
        }

        static async Task<Results<Ok, BadRequest>> ChangeLanguage(
            [FromBody] ChangeLanguageDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new ChangeLanguageCommand(dto);
            var success = await mediator.Send(command);

            return success ? TypedResults.Ok() : TypedResults.BadRequest();
        }

        static async Task<Results<Ok<UserDTO>, NotFound>> GetUser(
            [FromServices] IMediator mediator)
        {
            var query = new GetUserQuery();
            var user = await mediator.Send(query);

            return user is not null
                ? TypedResults.Ok(user)
                : TypedResults.NotFound();
        }

        static async Task<IResult> UpdateProfile(
            [FromForm] UpdateProfileNameDTO dto,
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] ILoggerFactory loggerFactory,
            [FromServices] IWebHostEnvironment env)
        {
            var logger = loggerFactory.CreateLogger("AuthsEndpoints");
            try
            {
                var command = new UpdateProfileCommand(dto);
                var response = await mediator.Send(command);

                if (response == false)
                    return TypedResults.BadRequest();

                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating profile");
                return TypedResults.Problem(
                    detail: env.IsDevelopment()
                        ? ex.Message
                        : "An error occurred while updating the profile.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}

