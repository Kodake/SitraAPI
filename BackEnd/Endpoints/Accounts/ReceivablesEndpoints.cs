using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class ReceivablesEndpoints
    {
        public static RouteGroupBuilder MapReceivables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"))
                .AddPaginationParamsOpenAPI();
            //.RequireAuthorization("isAdmin");
            group.MapGet("/{id:long}", GetAllById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"));
            group.MapGet("/{id:long}/viewPays", GetPaysById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"));
            //.RequireAuthorization("isAdmin");
            group.MapPost("/{id:long}/addPayment", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePaymentReceiptDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapDelete("/{id:long}/paymentReceipts/{paymentReceiptId}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<ReceivableListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var receivables = await unitOfWork.ReceivableRepository.GetAllAsync(pagination);
            var receivablesDTO = mapper.Map<List<ReceivableListDTO>>(receivables);

            return TypedResults.Ok(receivablesDTO);
        }

        static async Task<Results<Ok<ReceivableDTO>, NotFound>> GetAllById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromRoute] long id)
        {
            var receivable = await unitOfWork.ReceivableRepository.GetAllByIdAsync(id);

            if (receivable == null)
            {
                return TypedResults.NotFound();
            }

            var receivableDTO = mapper.Map<ReceivableDTO>(receivable);

            return TypedResults.Ok(receivableDTO);
        }

        static async Task<Results<Ok<ReceivableDTO>, NotFound>> GetPaysById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromRoute] long id)
        {
            var receivable = await unitOfWork.ReceivableRepository.GetPaysByIdAsync(id);

            if (receivable == null)
            {
                return TypedResults.NotFound();
            }

            var receivableDTO = mapper.Map<ReceivableDTO>(receivable);

            return TypedResults.Ok(receivableDTO);
        }

        static async Task<Results<Created<PaymentReceiptDTO>, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePaymentReceiptDTO dto,
            [FromRoute] long id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var paymentReceipt = mapper.Map<PaymentReceipt>(dto);

            var receivable = await unitOfWork.ReceivableRepository.GetByIdAsync(id);

            if (receivable == null)
            {
                return TypedResults.NotFound();
            }

            paymentReceipt.ReceivableId = id;
            await unitOfWork.PaymentReceiptRepository.CreateAsync(paymentReceipt);

            receivable.TotalOutstanding += paymentReceipt.Amount;

            receivable.Status = receivable.TotalOutstanding switch
            {
                var total when total >= receivable.TotalAmountDue => AccountStatus.Paid,
                var total when total > 0 => AccountStatus.PartiallyPaid,
                _ => receivable.Status
            };

            unitOfWork.ReceivableRepository.Update(receivable);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payment-receipts-get", default);
            await outputCacheStore.EvictByTagAsync("receivables-get", default);

            var paymentReceiptDTO = mapper.Map<PaymentReceiptDTO>(paymentReceipt);

            return TypedResults.Created($"/{paymentReceipt.Id}/addPayment", paymentReceiptDTO);
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromRoute] long paymentReceiptId,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IOutputCacheStore outputCacheStore)
        {
            var paymentReceipt = await unitOfWork.PaymentReceiptRepository.GetByIdAsync(paymentReceiptId);

            if (paymentReceipt == null)
            {
                return TypedResults.NotFound();
            }

            var receivable = await unitOfWork.ReceivableRepository.GetByIdAsync(id);

            if (receivable == null)
            {
                return TypedResults.NotFound();
            }

            receivable.TotalOutstanding -= paymentReceipt.Amount;

            if (receivable.TotalOutstanding < 0)
            {
                receivable.TotalOutstanding = 0;
            }

            receivable.Status = receivable.TotalOutstanding switch
            {
                0 => AccountStatus.PendingReceivable,
                var total when total < receivable.TotalAmountDue => AccountStatus.PartiallyPaid,
                _ => receivable.Status
            };

            await unitOfWork.PaymentReceiptRepository.DeactivateAsync(paymentReceiptId);

            unitOfWork.ReceivableRepository.Update(receivable);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payment-receipts-get", default);
            await outputCacheStore.EvictByTagAsync("receivables-get", default);

            return TypedResults.Ok();
        }

    }
}
