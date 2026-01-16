using AutoMapper;
using SITRA.Core.DTOs;
using MediatR;
using QuestPDF.Infrastructure;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetClosePettyCashPdfQuery : IQueryMediator<(byte[] FileBytes, string FileName)?>
    {
        public GetClosePettyCashPdfQuery() { }

        public async Task<(byte[] FileBytes, string FileName)?> GetData(
            IMediator mediator,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // 1?? Call existing command to get PettyCashDTO
            var pettyCashQuery = new GetPettyCashByUserIdQuery();
            var pettyCashDto = await mediator.Send(pettyCashQuery, cancellationToken);

            if (pettyCashDto == null)
                return null;

            // 2?? Generate PDF using ClosePettyCashDTO
            var closePettyCashDoc = new ClosePettyCashDTO(pettyCashDto);

            var fileName = $"ClosePettyCash_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            closePettyCashDoc.GeneratePdf(filePath);

            // 3?? Read file and return as bytes
            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var file = (fileBytes, fileName);

            return file;
        }
    }
}
