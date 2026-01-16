using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Appointments.Appointments
{
    public class DeactivateAppointmentCommand : ToggleCommand
    {
        public DeactivateAppointmentCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(Id);

            if (appointment == null) return false;

            await unitOfWork.AppointmentRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("appointments-get", cancellationToken);

            return true;
        }
    }
}
