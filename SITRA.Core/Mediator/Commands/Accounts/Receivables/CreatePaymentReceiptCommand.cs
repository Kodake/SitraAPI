using AutoMapper;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Accounts.Receivables
{
    public class CreatePaymentReceiptCommand : CreateCommand<PaymentReceipt, CreatePaymentReceiptDTO>, IIdempotentCommand
    {
        public Guid Id { get; set; }
        public CreatePaymentReceiptCommand(Guid id, CreatePaymentReceiptDTO dto) : base(dto)
        {
            Id = id;
        }

        public override async Task<CreatePaymentReceiptDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var paymentReceipt = mapper.Map<PaymentReceipt>(Dto);
            paymentReceipt.PaymentDate = DateTimeOffset.UtcNow;

            var receivable = await unitOfWork.ReceivableRepository.GetByIdAsync(Id);

            paymentReceipt.ReceivableId = Id;
            await unitOfWork.PaymentReceiptRepository.CreateAsync(paymentReceipt);

            receivable!.TotalOutstanding += paymentReceipt.Amount;

            receivable.Status = receivable.TotalOutstanding switch
            {
                var total when total >= receivable.TotalAmountDue => AccountStatus.Paid,
                var total when total > 0 => AccountStatus.PartiallyPaid,
                _ => receivable.Status
            };

            unitOfWork.ReceivableRepository.Update(receivable!);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("paymentReceipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);

            return Dto;
        }
    }
}
