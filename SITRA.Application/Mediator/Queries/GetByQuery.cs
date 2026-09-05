using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
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
