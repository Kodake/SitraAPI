using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Services;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Services
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
