using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Services
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
