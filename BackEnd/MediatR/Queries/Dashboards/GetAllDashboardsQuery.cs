using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Dashboards
{
    public class GetAllDashboardsQuery : GetSelectsQuery<DashboardListDTO>
    {
        public GetAllDashboardsQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<DashboardListDTO>> GetData(
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

            var dashboard = new DashboardListDTO
            {
                TotalWarehouses = totalWarehouses,
                TotalContactsAdded = totalContactsAdded,
                TotalSaleInvoicesEmitted = totalSaleInvoicesEmitted,
                TotalProductsInStock = totalProductsInStock,
                Last30DaysSales = last30DaysSales,
                Last30DaysPurchases = last30DaysPurchases
            };

            return [dashboard];
        }
    }
}