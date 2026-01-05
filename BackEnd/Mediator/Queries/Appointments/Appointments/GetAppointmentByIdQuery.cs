using AutoMapper;
using BackEnd.DTOs.Appointments.Appointments;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Appointments.Appointments
{
    public class GetAppointmentByIdQuery : GetByIdQuery<AppointmentDTO>
    {
        public GetAppointmentByIdQuery(long id) : base(id) { }

        public override async Task<AppointmentDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(Id);
            return mapper.Map<AppointmentDTO>(appointment);
        }
    }
}
