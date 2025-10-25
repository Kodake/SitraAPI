using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Entities.CashBoxes;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;
using QuestPDF.Infrastructure;

namespace BackEnd.Mediator.Commands.CashBoxes
{
    public class UpdateClosePettyCashCommand(long id, CreateClosePettyCashDTO dto)
        : UpdateFileCommand<PettyCash, CreateClosePettyCashDTO>(id, dto)
    {
        public override async Task<(byte[] FileBytes, string FileName)?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // 🔐 Ensure user is authenticated
            var user = await unitOfWork.UserService.GetUserAsync()
                ?? throw new UnauthorizedAccessException("No authenticated user found.");

            // 📦 Get the current open petty cash
            var pettyCashDB = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
            if (pettyCashDB == null || pettyCashDB.UserId != user.Id)
                return null;

            // 📝 Update fields
            pettyCashDB.Comments = Dto.Comments;
            pettyCashDB.ClosingDate = DateTime.UtcNow;
            pettyCashDB.Status = PettyCashStatus.Closed;

            // 💰 Compute movement totals
            var paymentMethods = await unitOfWork.PettyCashMovementRepository
                .GetPaymentMethodSumsAsync(pettyCashDB.Id);
            var totalWithdrawals = await unitOfWork.PettyCashMovementRepository
                .GetTotalWithdrawalsAsync(pettyCashDB.Id);

            // 🧾 Map to DTO
            var pettyCashDto = mapper.Map<PettyCashDTO>(pettyCashDB);

            pettyCashDto.PaymentMethods = paymentMethods
                .Where(kvp => kvp.Key is PaymentMethod)
                .Select(kvp => new PaymentMethodAmountListDTO
                {
                    Method = (PaymentMethod)kvp.Key,
                    Amount = kvp.Value
                })
                .ToList();

            pettyCashDto.CashBilled = paymentMethods.TryGetValue(PaymentMethod.Cash, out var cash)
                ? cash
                : 0;

            pettyCashDto.TotalBilled = pettyCashDto.PaymentMethods.Sum(p => p.Amount);
            pettyCashDto.WithdrawalAmount = totalWithdrawals;
            pettyCashDto.ClosingAmount =
                pettyCashDto.OpeningAmount + pettyCashDto.CashBilled - pettyCashDto.WithdrawalAmount;

            pettyCashDB.ClosingAmount = pettyCashDto.ClosingAmount;

            // 💾 Save updates
            unitOfWork.PettyCashRepository.Update(pettyCashDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            // 📄 Generate PDF and return it
            var pdfDoc = new ClosePettyCashDTO(pettyCashDto);
            var fileName = $"ClosePettyCash_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            pdfDoc.GeneratePdf(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

            return (fileBytes, fileName);
        }
    }
}
