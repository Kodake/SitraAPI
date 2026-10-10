using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Accounts.Payables
{
    public class DeactivatePayableCommand(Guid id, Guid payableReceiptId) : ToggleCommand(id), IIdempotentCommand
    {
        public Guid PayableReceiptId { get; set; } = payableReceiptId;

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
