using MediatR;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class ChangeLanguageCommand : IRequest<bool>
    {
        public ChangeLanguageDTO Dto { get; }

        public ChangeLanguageCommand(ChangeLanguageDTO dto)
        {
            Dto = dto;
        }
    }
}
