using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;
using System.Linq;

namespace SITRA.Infrastructure.Services.Products
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Item, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive && u.ItemType == ItemType.Service,
                StatusType.Deactivated => u => !u.IsActive && u.ItemType == ItemType.Service,
                _ => u => u.ItemType == ItemType.Service
            };

            var items = await _unitOfWork.ServiceRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }
    }
}
