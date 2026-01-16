using AutoMapper;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Services
{
    public class GetSelectsServicesQuery : GetSelectsQuery<ServiceListDTO>
    {
        public GetSelectsServicesQuery(StatusType status) : base(status) { }

        public override async Task<List<ServiceListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var services = await unitOfWork.ServiceRepository.GetAll(Status);
            return mapper.Map<List<ServiceListDTO>>(services);
        }
    }
}
