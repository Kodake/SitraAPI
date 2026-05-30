using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.Products
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Brand>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Brand, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var brands = await _unitOfWork.BrandRepository.GetAllAsync(filter, cancellationToken);
            return brands.OrderBy(u => u.Id).ToList();
        }
    }
}
