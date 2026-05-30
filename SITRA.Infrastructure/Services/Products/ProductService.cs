using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;
using System.Linq;

namespace SITRA.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Item, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive && u.ItemType == ItemType.Product,
                StatusType.Deactivated => u => !u.IsActive && u.ItemType == ItemType.Product,
                _ => u => u.ItemType == ItemType.Product
            };

            var items = await _unitOfWork.ProductRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<IEnumerable<Item>> GetStockProductsAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.ProductRepository.GetAllAsync(p => p.ItemType == ItemType.Product && p.Stocks.Any(x => x.Item.ItemType == ItemType.Product), cancellationToken);
            return items.Select(p => new Item
            {
                Id = p.Id,
                Name = p.Name,
                Reference = p.Reference,
                Barcode = p.Barcode,
            });
        }
    }
}
