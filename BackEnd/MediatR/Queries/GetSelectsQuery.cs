using AutoMapper;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries
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
