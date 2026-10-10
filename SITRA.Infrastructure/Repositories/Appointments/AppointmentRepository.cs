using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Appointments;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Appointments
{
    public class AppointmentRepository : RepositoryBase<Appointment>, IAppointmentRepository
    {
        private new readonly HttpContext httpContext;
        public AppointmentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }


        public async Task<bool> ExistsAtDateTime(DateTimeOffset scheduledAt, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await context.Appointments.AnyAsync(a =>
                a.IsActive &&
                a.ScheduledAt.Year == scheduledAt.Year &&
                a.ScheduledAt.Month == scheduledAt.Month &&
                a.ScheduledAt.Day == scheduledAt.Day &&
                a.ScheduledAt.Hour == scheduledAt.Hour &&
                a.ScheduledAt.Minute == scheduledAt.Minute &&
                (!excludeId.HasValue || a.Id != excludeId.Value), cancellationToken
            );
        }

        public async new Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Appointments
                .Include(a => a.Contact)
                .Include(a => a.AppointmentItems)
                .ThenInclude(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
    }
}





