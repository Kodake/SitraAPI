using AutoMapper;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Entities.CashBoxes;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.CashBoxes
{
    public class CreateOpenPettyCashCommand(CreateOpenPettyCashDTO dto) : CreateCommand<PettyCash, CreateOpenPettyCashDTO>(dto)
    {
        public override async Task<CreateOpenPettyCashDTO> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync()
                ?? throw new UnauthorizedAccessException("No user authenticated found.");

            var otherOpenPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
            if (otherOpenPettyCash != null)
            {
                throw new Exception($"Petty cash is currently opened by {otherOpenPettyCash.User.Email}. Please wait until it is closed.");
            }

            var existingUserPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
            if (existingUserPettyCash != null)
            {
                throw new Exception("You already have an open petty cash. Please close it before opening a new one.");
            }

            var pettyCash = new PettyCash
            {
                UserId = user.Id,
                OpeningAmount = Dto.OpeningAmount,
                ClosingAmount = Dto.OpeningAmount,
                OpeningDate = DateTime.UtcNow,
                Status = PettyCashStatus.Open,
            };

            await unitOfWork.PettyCashRepository.CreateAsync(pettyCash);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            return Dto;
        }
    }
}