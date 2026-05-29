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

        public async new Task<List<Appointment>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Appointments
                .OrderBy(x => x.Description);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Contact)
                .Include(p => p.AppointmentItems)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async new Task<List<Appointment>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Appointments.AsNoTracking()
                .OrderBy(x => x.Description);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Contact)
                .Include(p => p.AppointmentItems)
                .ToListAsync(cancellationToken);

            return items;
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




