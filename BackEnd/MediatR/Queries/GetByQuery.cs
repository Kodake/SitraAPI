using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries
{
    public abstract class GetByQuery<TDto> : IQuery<TDto>
    {
        protected GetByQuery()
        { }

        public abstract Task<TDto> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
