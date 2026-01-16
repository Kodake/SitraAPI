using SITRA.Core.Entities.Auths;
using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.CashBoxes
{
    public class CreateWithdrawalPettyCashMovementCommand(CreateWithdrawalPettyCashMovementDTO dto)
    : CreateCommand<PettyCashMovement, CreateWithdrawalPettyCashMovementDTO>(dto)
    {
        public override async Task<CreateWithdrawalPettyCashMovementDTO> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync()
                ?? throw new UnauthorizedAccessException("No user authenticated found.");

            var otherOpenPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
            if (otherOpenPettyCash != null && otherOpenPettyCash.UserId != user.Id)
            {
                throw new Exception($"Petty cash is currently opened by {otherOpenPettyCash.User.Email}. Please wait until it is closed.");
            }

            var pettyCashDB = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id)
                ?? throw new Exception("You don't have an open petty cash. Please open one before making a withdrawal.");

            var movement = new PettyCashMovement
            {
                PettyCashId = pettyCashDB.Id,
                Amount = Dto.WithdrawalAmount,
                Type = MovementType.CashWithdrawals,
                CreatedAt = DateTime.UtcNow,
                InvoiceId = null,
                Invoice = null,
            };

            await unitOfWork.PettyCashMovementRepository.CreateAsync(movement);

            var totalWithdrawals = await unitOfWork.PettyCashMovementRepository
                .GetTotalWithdrawalsAsync(pettyCashDB.Id);

            pettyCashDB.ClosingAmount -= Dto.WithdrawalAmount;
            pettyCashDB.WithdrawalAmount = totalWithdrawals;

            await unitOfWork.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);

            return Dto;
        }
    }
}

