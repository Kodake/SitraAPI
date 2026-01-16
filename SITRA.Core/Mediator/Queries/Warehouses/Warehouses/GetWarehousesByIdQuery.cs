using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
{
    public class GetWarehousesByIdQuery : GetByIdQuery<WarehouseDTO>
    {
        public GetWarehousesByIdQuery(long id) : base(id) { }

        public override async Task<WarehouseDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(Id);
            return mapper.Map<WarehouseDTO>(warehouse);
        }
    }
}
