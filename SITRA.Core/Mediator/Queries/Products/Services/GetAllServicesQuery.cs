using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Services
{
    public class GetAllServicesQuery : GetAllQuery<ServiceListDTO>
    {
        public GetAllServicesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ServiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var services = await unitOfWork.ServiceRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ServiceListDTO>>(services);
        }
    }
}
