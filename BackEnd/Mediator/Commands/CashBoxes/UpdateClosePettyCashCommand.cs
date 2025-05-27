using AutoMapper;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Entities.CashBoxes;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.CashBoxes
{
    public class UpdateClosePettyCashCommand(long id, CreateClosePettyCashDTO dto) : UpdateCommand<PettyCash, CreateClosePettyCashDTO>(id, dto)
    {
        public override async Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync()
                ?? throw new UnauthorizedAccessException("No user authenticated found.");

            var pettyCashDB = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);

            if (pettyCashDB == null || pettyCashDB.UserId != user.Id)
                return false;

            pettyCashDB.Comments = Dto.Comments;
            pettyCashDB.ClosingDate = DateTime.UtcNow;
            pettyCashDB.Status = PettyCashStatus.Closed;

            //var paymentMethods = await unitOfWork.PettyCashMovementRepository
            //    .GetPaymentMethodSumsAsync(pettyCashDB.Id);

            //var totalWithdrawals = await unitOfWork.PettyCashMovementRepository
            //    .GetTotalWithdrawalsAsync(pettyCashDB.Id);

            //var totalBilled = paymentMethods.TryGetValue("Total", out var total)
            //    ? total
            //    : paymentMethods
            //        .Where(kvp => kvp.Key is PaymentMethod)
            //        .Sum(kvp => kvp.Value);

            //var cashBilled = paymentMethods.TryGetValue(PaymentMethod.Cash, out var cash)
            //    ? cash
            //    : 0;

            //pettyCashDB.ClosingAmount = pettyCashDB.OpeningAmount + cashBilled - totalWithdrawals;

            unitOfWork.PettyCashRepository.Update(pettyCashDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            return true;
        }

    }
}
