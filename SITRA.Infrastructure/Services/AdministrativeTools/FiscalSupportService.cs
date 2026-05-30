using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.AdministrativeTools
{
    public class FiscalSupportService : IFiscalSupportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FiscalSupportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FiscalSupport>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<FiscalSupport, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.FiscalSupportRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }
    }
}
