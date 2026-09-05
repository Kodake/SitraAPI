using AutoMapper;
using Mediator;
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

    public class GetClosePettyCashPdfQueryHandler : IRequestHandler<GetClosePettyCashPdfQuery, (byte[] FileBytes, string FileName)?>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetClosePettyCashPdfQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async ValueTask<(byte[] FileBytes, string FileName)?> Handle(GetClosePettyCashPdfQuery request, CancellationToken cancellationToken)
        {
            return await request.GetData(_mediator, _mapper, cancellationToken);
        }
    }
}
