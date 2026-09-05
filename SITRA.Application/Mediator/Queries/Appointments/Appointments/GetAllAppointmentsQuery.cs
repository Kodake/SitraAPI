using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Interfaces.Services.Appointments;

namespace SITRA.Core.Mediator.Queries.Appointments.Appointments
{
    public class GetAllAppointmentsQuery : IRequest<List<AppointmentListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllAppointmentsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentListDTO>>
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public GetAllAppointmentsQueryHandler(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        public async ValueTask<List<AppointmentListDTO>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<AppointmentListDTO>>(appointments);
        }
    }
}
