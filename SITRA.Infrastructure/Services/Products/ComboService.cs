using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;
using System.Linq;

namespace SITRA.Infrastructure.Services.Products
{
    public class ComboService : IComboService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ComboService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Item, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive && u.ItemType == ItemType.Combo,
                StatusType.Deactivated => u => !u.IsActive && u.ItemType == ItemType.Combo,
                _ => u => u.ItemType == ItemType.Combo
            };

            var items = await _unitOfWork.ComboRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<IEnumerable<Item>> GetStockCombosAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.ComboRepository.GetAllAsync(p => p.ItemType == ItemType.Combo && p.Stocks.Any(x => x.Item.ItemType == ItemType.Combo), cancellationToken);
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
