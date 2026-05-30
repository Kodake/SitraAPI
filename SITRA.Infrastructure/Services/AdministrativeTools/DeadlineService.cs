using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.AdministrativeTools
{
    public class DeadlineService : IDeadlineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeadlineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Deadline>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Deadline, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.DeadlineRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }
    }
}
