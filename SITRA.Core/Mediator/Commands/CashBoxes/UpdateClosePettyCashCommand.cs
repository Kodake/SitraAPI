using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.CashBoxes
{
    public class UpdateClosePettyCashCommand(Guid id, CreateClosePettyCashDTO dto)
        : UpdateFileCommand<PettyCash, CreateClosePettyCashDTO>(id, dto)
    {
        public override async Task<(byte[] FileBytes, string FileName)?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // All validations are handled by FluentValidation before this action is executed
            var user = await unitOfWork.UserService.GetUserAsync();

            // ?? Get the current open petty cash
            var pettyCashDB = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user!.Id);

            // ?? Update fields
            pettyCashDB.Comments = Dto.Comments;
            pettyCashDB.ClosingDate = DateTimeOffset.UtcNow;
            pettyCashDB.Status = PettyCashStatus.Closed;

            // ?? Compute movement totals
            var paymentMethods = await unitOfWork.PettyCashMovementRepository
                .GetPaymentMethodSumsAsync(pettyCashDB.Id);
            var totalWithdrawals = await unitOfWork.PettyCashMovementRepository
                .GetTotalWithdrawalsAsync(pettyCashDB.Id);

            // ?? Map to DTO
            var pettyCashDto = mapper.Map<PettyCashDTO>(pettyCashDB);

            pettyCashDto.PaymentMethods = paymentMethods
                .Where(kvp => kvp.Key is PaymentMethod)
                .Select(kvp => new PaymentMethodAmountListDTO
                {
                    MethodType = (PaymentMethod)kvp.Key,
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

            // ?? Save updates
            unitOfWork.PettyCashRepository.Update(pettyCashDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            // ?? Generate PDF and return it
            var pdfDoc = new ClosePettyCashDocument(pettyCashDto);
            var fileName = $"ClosePettyCash_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            pdfDoc.GeneratePdf(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

            return (fileBytes, fileName);
        }
    }
}

