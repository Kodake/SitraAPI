using AutoMapper;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
{
    public abstract class GetSelectsQuery<TDto> : IQuery<List<TDto>>
    {
        public StatusType Status { get; set; }

        protected GetSelectsQuery(StatusType status)
        {
            Status = status;
        }

        public abstract Task<List<TDto>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
