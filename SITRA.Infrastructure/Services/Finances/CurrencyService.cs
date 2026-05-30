using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Finances;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.Finances
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CurrencyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Currency>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Currency, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.CurrencyRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }
    }
}
