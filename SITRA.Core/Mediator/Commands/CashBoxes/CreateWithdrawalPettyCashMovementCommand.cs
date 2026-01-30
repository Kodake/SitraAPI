using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

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
            // All validations are handled by FluentValidation
            // These calls are now safe because validation ensures they exist
            var user = await unitOfWork.UserService.GetUserAsync();
            var pettyCashDB = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user!.Id);

            var movement = new PettyCashMovement
            {
                PettyCashId = pettyCashDB!.Id,
                Amount = Dto.WithdrawalAmount,
                Type = MovementType.CashWithdrawals,
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

