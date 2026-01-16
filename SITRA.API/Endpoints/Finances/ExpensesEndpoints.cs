using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Finances.Expenses;
using SITRA.Core.Mediator.Queries.Finances.Expenses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SITRA.API.Endpoints
{
    public static class ExpensesEndpoints
    {
        public static RouteGroupBuilder MapExpenses(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("expenses-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateExpenseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateExpenseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ExpenseListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllExpensesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ExpenseDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetExpenseByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateExpenseDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateExpenseCommand(dto);
            var taxDTO = await mediator.Send(command);
            return taxDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateExpenseDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateExpenseCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateExpenseCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateExpenseCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

