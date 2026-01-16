using AutoMapper;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Appointments.Appointments
{
    public class CreateAppointmentCommand : CreateCommand<Appointment, CreateAppointmentDTO>
    {
        public CreateAppointmentCommand(CreateAppointmentDTO dto) : base(dto) { }

        public override async Task<CreateAppointmentDTO> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var appointment = mapper.Map<Appointment>(Dto);

            appointment.AppointmentItems = Dto.AppointmentItemIds.Select(itemId => new AppointmentItem { ItemId = itemId }).ToList();

            await unitOfWork.AppointmentRepository.CreateAsync(appointment);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("appointments-get", cancellationToken);
            return Dto;
        }

    }
}
