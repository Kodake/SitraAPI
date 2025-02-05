using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Accounts.Payables
{
    public class DeactivatePayableCommand : ToggleCommand
    {
        public long PayableReceiptId { get; set; }
        public DeactivatePayableCommand(long id, long payableReceiptId) : base(id)
        {
            PayableReceiptId = payableReceiptId;
        }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var payableReceipt = await unitOfWork.PayableReceiptRepository.GetByIdAsync(PayableReceiptId);

            if (payableReceipt == null)
            {
                return false;
            }

            var payable = await unitOfWork.PayableRepository.GetByIdAsync(Id);

            if (payable == null)
            {
                return false;
            }

            payable.TotalOutstanding -= payableReceipt.Amount;

            if (payable.TotalOutstanding < 0)
            {
                payable.TotalOutstanding = 0;
            }

            payable.Status = payable.TotalOutstanding switch
            {
                0 => AccountStatus.PendingPayable,
                var total when total < payable.TotalAmountDue => AccountStatus.PartiallyPaid,
                _ => payable.Status
            };

            await unitOfWork.PayableReceiptRepository.DeactivateAsync(PayableReceiptId);

            unitOfWork.PayableRepository.Update(payable);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payableReceipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("payables-get", cancellationToken);

            return true;
        }
    }
}
