using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Services
{
    public class GetServiceByIdQuery : GetByIdQuery<ServiceDTO>
    {
        public GetServiceByIdQuery(long id) : base(id) { }

        public override async Task<ServiceDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var service = await unitOfWork.ServiceRepository.GetByIdAsync(Id);
            return mapper.Map<ServiceDTO>(service);
        }
    }
}
