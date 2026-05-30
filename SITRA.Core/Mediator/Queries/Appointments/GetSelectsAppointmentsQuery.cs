using SITRA.Core.Interfaces.Services.Appointments;
using MediatR;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Appointments
{
    public class GetSelectsAppointmentsQuery : IRequest<List<AppointmentListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsAppointmentsQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsAppointmentsQueryHandler : IRequestHandler<GetSelectsAppointmentsQuery, List<AppointmentListDTO>>
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public GetSelectsAppointmentsQueryHandler(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        public async Task<List<AppointmentListDTO>> Handle(GetSelectsAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<AppointmentListDTO>>(appointments);
        }
    }
}
