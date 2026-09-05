using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
{
    public class GetSelectsWarehousesQuery : GetSelectsQuery<WarehouseListDTO>
    {
        public GetSelectsWarehousesQuery(StatusType status) : base(status) { }

        public override async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetAll(Status);
            return mapper.Map<List<WarehouseListDTO>>(warehouses);
        }
    }
}
