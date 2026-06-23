using SITRA.Core.Entities.Appointments;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Appointments
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Appointment>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
