using AutoMapper;
using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Accounts.Receivables
{
    public class CreatePaymentReceiptCommand : CreateCommand<PaymentReceipt, CreatePaymentReceiptDTO>
    {
        public long Id { get; set; }
        public CreatePaymentReceiptCommand(long id, CreatePaymentReceiptDTO dto) : base(dto)
        {
            Id = id;
        }

        public override async Task<CreatePaymentReceiptDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var paymentReceipt = mapper.Map<PaymentReceipt>(Dto);

            var receivable = await unitOfWork.ReceivableRepository.GetByIdAsync(Id)
                ?? throw new InvalidOperationException("Receivable not found.");

            paymentReceipt.ReceivableId = Id;
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
            await outputCacheStore.EvictByTagAsync("payment-receipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);

            return Dto;
        }
    }
}
