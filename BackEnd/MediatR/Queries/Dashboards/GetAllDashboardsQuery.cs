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
            var totalWarehouses = await unitOfWork.WarehouseRepository.TotalWarehousesAddedAsync();
            var totalContactsAdded = await unitOfWork.ContactRepository.TotalContactsAddedAsync();
            var last24HoursSaleInvoices = await unitOfWork.SaleInvoiceRepository.Last24HoursSaleInvoicesAsync();
            var totalProductsInStock = await unitOfWork.StockRepository.TotalProductsInStockAsync();
            var last30DaysSales = await unitOfWork.SaleInvoiceRepository.Last30DaysSalesAsync();
            var last30DaysPurchases = await unitOfWork.PurchaseInvoiceRepository.Last30DaysPurchasesAsync();

            var dashboard = new DashboardDTO
            {
                TotalWarehouses = totalWarehouses,
                TotalContactsAdded = totalContactsAdded,
                Last24HoursSaleInvoices = last24HoursSaleInvoices,
                TotalProductsInStock = totalProductsInStock,
                Last30DaysSales = last30DaysSales,
                Last30DaysPurchases = last30DaysPurchases
            };

            return dashboard;
        }
    }
}