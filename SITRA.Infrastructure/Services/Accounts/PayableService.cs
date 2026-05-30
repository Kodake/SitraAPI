using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Accounts;

namespace SITRA.Infrastructure.Services.Accounts
{
    public class PayableService : IPayableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PayableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Payable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.PayableRepository.GetAllByIdAsync(id, cancellationToken);
            if (item != null && item.Status == AccountStatus.Paid)
            {
                item.PayableReceipts = item.PayableReceipts.Where(x => x.IsActive).OrderByDescending(p => p.Id).ToList();
                return item;
            }
            return null;
        }
    }
}
