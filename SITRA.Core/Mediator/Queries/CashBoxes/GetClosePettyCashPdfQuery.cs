using AutoMapper;
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
            // 1?? Call existing command to get PettyCashDTO
            var pettyCashQuery = new GetPettyCashByUserIdQuery();
            var pettyCashDto = await mediator.Send(pettyCashQuery, cancellationToken);

            if (pettyCashDto == null)
                return null;

            // 2?? Generate PDF using ClosePettyCashDocument
            var closePettyCashDoc = new SITRA.Core.Documents.ClosePettyCashDocument(pettyCashDto);

            var fileName = $"ClosePettyCash_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}.pdf";
            
            var fileBytes = closePettyCashDoc.GenerateToBytes();

            var file = (fileBytes, fileName);

            return file;
        }
    }
}
