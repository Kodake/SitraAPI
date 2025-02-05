using AutoMapper;
using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Accounts.Payables
{
    public class CreatePayableReceiptCommand : CreateCommand<PayableReceipt, CreatePayableReceiptDTO>
    {
        public long Id { get; set; }
        public CreatePayableReceiptCommand(long id, CreatePayableReceiptDTO dto) : base(dto)
        {
            Id = id;
        }

        public override async Task<CreatePayableReceiptDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var payableReceipt = mapper.Map<PayableReceipt>(Dto);

            var payable = await unitOfWork.PayableRepository.GetByIdAsync(Id)
                ?? throw new InvalidOperationException("Payable not found.");

            payableReceipt.PayableId = Id;
            await unitOfWork.PayableReceiptRepository.CreateAsync(payableReceipt);

            payable.TotalOutstanding += payableReceipt.Amount;

            payable.Status = payable.TotalOutstanding switch
            {
                var total when total >= payable.TotalAmountDue => AccountStatus.Paid,
                var total when total > 0 => AccountStatus.PartiallyPaid,
                _ => payable.Status
            };

            unitOfWork.PayableRepository.Update(payable);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payableReceipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("payables-get", cancellationToken);

            return Dto;
        }
    }
}
