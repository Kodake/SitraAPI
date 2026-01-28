using SITRA.Core.Entities.Appointments;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Appointments
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<bool> ExistsAtDateTime(DateTimeOffset scheduledAt, Guid? excludeId = null);
    }
}
