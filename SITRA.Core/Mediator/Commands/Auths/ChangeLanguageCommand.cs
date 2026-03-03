using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class ChangeLanguageCommand : AuthCommand<bool>
    {
        public ChangeLanguageDTO Dto { get; }

        public ChangeLanguageCommand(ChangeLanguageDTO dto)
        {
            Dto = dto;
        }

        public override Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserService.ChangeLanguageAsync(Dto.Language, cancellationToken);
        }
    }
}
