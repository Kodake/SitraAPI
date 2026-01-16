using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Appointments.Appointments
{
    public class GetAllAppointmentsQuery : GetAllQuery<AppointmentListDTO>
    {
        public GetAllAppointmentsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<AppointmentListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var appointments = await unitOfWork.AppointmentRepository.GetAllAsync(Pagination);
            return mapper.Map<List<AppointmentListDTO>>(appointments);
        }
    }
}
