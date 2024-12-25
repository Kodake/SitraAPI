using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries
{
    public abstract class GetByIdQuery<TDto> : IQuery<TDto>
    {
        public long Id { get; set; }

        protected GetByIdQuery(long id)
        {
            Id = id;
        }

        public abstract Task<TDto> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
