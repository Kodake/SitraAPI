using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Dashboards
{
    public class GetAllDashboardsQuery : GetByQuery<DashboardDTO>
    {
        public GetAllDashboardsQuery() : base() { }

        public override async Task<DashboardDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var totalWarehouses = await unitOfWork.WarehouseRepository.TotalWarehousesAdded();
            var totalContactsAdded = await unitOfWork.ContactRepository.TotalContactsAdded();
            var totalSaleInvoicesEmitted = await unitOfWork.SaleInvoiceRepository.TotalSaleInvoicesEmitted();
            var totalProductsInStock = await unitOfWork.StockRepository.TotalProductsInStock();
            var last30DaysSales = await unitOfWork.SaleInvoiceRepository.Last30DaysSales();
            var last30DaysPurchases = await unitOfWork.PurchaseInvoiceRepository.Last30DaysPurchases();

            var dashboard = new DashboardDTO
            {
                TotalWarehouses = totalWarehouses,
                TotalContactsAdded = totalContactsAdded,
                TotalSaleInvoicesEmitted = totalSaleInvoicesEmitted,
                TotalProductsInStock = totalProductsInStock,
                Last30DaysSales = last30DaysSales,
                Last30DaysPurchases = last30DaysPurchases
            };

            return dashboard;
        }
    }
}