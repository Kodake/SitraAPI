using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.Products
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Category>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Category, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(filter, cancellationToken);
            return categories.OrderBy(u => u.Id).ToList();
        }
    }
}
