using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Accounts;

namespace SITRA.Infrastructure.Services.Accounts
{
    public class ReceivableService : IReceivableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceivableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Receivable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.ReceivableRepository.GetAllByIdAsync(id, cancellationToken);
            if (item != null && item.Status == AccountStatus.Paid)
            {
                item.PaymentReceipts = item.PaymentReceipts.Where(x => x.IsActive).OrderByDescending(p => p.Id).ToList();
                return item;
            }
            return null;
        }
    }
}
