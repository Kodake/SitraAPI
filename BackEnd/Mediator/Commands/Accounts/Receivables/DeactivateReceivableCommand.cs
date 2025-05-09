using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Accounts.Receivables
{
    public class DeactivateReceivableCommand : ToggleCommand
    {
        public long PaymentReceiptId { get; set; }
        public DeactivateReceivableCommand(long id, long paymentReceiptId) : base(id)
        {
            PaymentReceiptId = paymentReceiptId;
        }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var paymentReceipt = await unitOfWork.PaymentReceiptRepository.GetByIdAsync(PaymentReceiptId);

            if (paymentReceipt == null)
            {
                return false;
            }

            var receivable = await unitOfWork.ReceivableRepository.GetByIdAsync(Id);

            if (receivable == null)
            {
                return false;
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

            await unitOfWork.PaymentReceiptRepository.DeactivateAsync(PaymentReceiptId);

            unitOfWork.ReceivableRepository.Update(receivable);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("payment-receipts-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("receivables-get", cancellationToken);

            return true;
        }
    }
}
