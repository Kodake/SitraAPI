using AutoMapper;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Accounts.Payables
{
    public class CreatePayableReceiptCommand(long id, CreatePayableReceiptDTO dto) : CreateCommand<PayableReceipt, CreatePayableReceiptDTO>(dto), IIdempotentCommand
    {
        public long Id { get; set; } = id;

        public override async Task<CreatePayableReceiptDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var payableReceipt = mapper.Map<PayableReceipt>(Dto);
            payableReceipt.PaymentDate = DateTimeOffset.UtcNow;

            var payable = await unitOfWork.PayableRepository.GetByIdAsync(Id);

            payableReceipt.PayableId = Id;
            await unitOfWork.PayableReceiptRepository.CreateAsync(payableReceipt);

            payable!.TotalOutstanding += payableReceipt.Amount;

            payable.Status = payable.TotalOutstanding switch
            {
                var total when total >= payable.TotalAmountDue => AccountStatus.Paid,
                var total when total > 0 => AccountStatus.PartiallyPaid,
                _ => payable.Status
            };

            unitOfWork.PayableRepository.Update(payable!);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payableReceipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("payables-get", cancellationToken);

            return Dto;
        }
    }
}
