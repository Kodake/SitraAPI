using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts;
using BackEnd.Entities.Accounts;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Accounts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class PaymentReceiptsEndpoints
    {
        public static RouteGroupBuilder MapPaymentReceipts(this RouteGroupBuilder group)
        {
            group.MapGet("/{receivableId:long}", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payment-receipts-get"))
                .AddPaginationParamsOpenAPI();
            //.RequireAuthorization("isAdmin");

            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePaymentReceiptDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<PaymentReceiptListDTO>>> GetAll(IPaymentReceiptRepository repository,
            IMapper mapper, [FromRoute] int receivableId, PaginationDTO paginacion)
        {
            var paymentReceipts = await repository.GetAll(paginacion, receivableId);
            var paymentReceiptsDTO = mapper.Map<List<PaymentReceiptListDTO>>(paymentReceipts);
            return TypedResults.Ok(paymentReceiptsDTO);
        }

        static async Task<Results<Created<PaymentReceiptDTO>, ValidationProblem>> Create([FromBody] CreatePaymentReceiptDTO dto,
           IPaymentReceiptRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var paymentReceipt = mapper.Map<PaymentReceipt>(dto);

            var id = await repository.Create(paymentReceipt);
            await outputCacheStore.EvictByTagAsync("payment-receipts-get", default);
            await outputCacheStore.EvictByTagAsync("receivables-get", default);
            var paymentReceiptDTO = mapper.Map<PaymentReceiptDTO>(paymentReceipt);
            return TypedResults.Created($"/paymentReceipts/{id}", paymentReceiptDTO);
        }
    }
}
