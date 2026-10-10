using AutoMapper;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Appointments.Appointments
{
    public class GetAppointmentByIdQuery : GetByIdQuery<AppointmentDTO>
    {
        public GetAppointmentByIdQuery(Guid id) : base(id) { }

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
