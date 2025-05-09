using AutoMapper;
using BackEnd.Repositories;
using BackEnd.DTOs.CashBoxes;

namespace BackEnd.Mediator.Queries.CashBoxes
{
    public class GetPettyCashByUserIdQuery : GetByQuery<PettyCashDTO?>
    {
        public GetPettyCashByUserIdQuery() : base() { }

        public override async Task<PettyCashDTO?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync()
                ?? throw new UnauthorizedAccessException("No user authenticated found.");

            var pettyCash = await unitOfWork.PettyCashRepository.GetAllByUserIdAsync(user.Id);
            return mapper.Map<PettyCashDTO?>(pettyCash);
        }
    }
}