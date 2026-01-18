using SITRA.Core.Entities.Auths;
using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.CashBoxes
{
    public class CreateOpenPettyCashCommand(CreateOpenPettyCashDTO dto) : CreateCommand<PettyCash, CreateOpenPettyCashDTO>(dto)
    {
        public override async Task<CreateOpenPettyCashDTO> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation
            // These calls are now safe because validation ensures they exist
            var user = await unitOfWork.UserService.GetUserAsync();

            var pettyCash = new PettyCash
            {
                UserId = user!.Id,
                OpeningAmount = Dto.OpeningAmount,
                ClosingAmount = Dto.OpeningAmount,
                OpeningDate = DateTimeOffset.UtcNow,
                Status = PettyCashStatus.Open,
            };

            await unitOfWork.PettyCashRepository.CreateAsync(pettyCash);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            return Dto;
        }
    }
}

