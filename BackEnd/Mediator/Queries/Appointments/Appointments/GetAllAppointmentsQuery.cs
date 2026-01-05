using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Appointments.Appointments;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Appointments.Appointments
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
