using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Appointments.Appointments;
using SITRA.Core.Mediator.Queries.Appointments.Appointments;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SITRA.API.Endpoints
{
    public static class AppointmentsEndpoints
    {
        public static RouteGroupBuilder MapAppointments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("appointments-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateAppointmentDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateAppointmentDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<AppointmentListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllAppointmentsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<AppointmentDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetAppointmentByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateAppointmentDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateAppointmentCommand(dto);
            var appointmentDTO = await mediator.Send(command);
            return appointmentDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateAppointmentDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateAppointmentCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateAppointmentCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateAppointmentCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

