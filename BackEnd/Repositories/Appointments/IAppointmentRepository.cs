using BackEnd.Entities.Appointments;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Appointments
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<bool> ExistsAtDateTime(DateTimeOffset scheduledAt, long? excludeId = null);
    }
}
