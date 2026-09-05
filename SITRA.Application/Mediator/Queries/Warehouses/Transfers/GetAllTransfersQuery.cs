using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Interfaces.Services.Warehouses;

namespace SITRA.Core.Mediator.Queries.Warehouses.Transfers
{
    public class GetAllTransfersQuery : IRequest<List<TransferListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllTransfersQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllTransfersQueryHandler : IRequestHandler<GetAllTransfersQuery, List<TransferListDTO>>
    {
        private readonly ITransferService _transferService;
        private readonly IMapper _mapper;

        public GetAllTransfersQueryHandler(ITransferService transferService, IMapper mapper)
        {
            _transferService = transferService;
            _mapper = mapper;
        }

        public async ValueTask<List<TransferListDTO>> Handle(GetAllTransfersQuery request, CancellationToken cancellationToken)
        {
            var transfers = await _transferService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<TransferListDTO>>(transfers);
        }
    }
}
