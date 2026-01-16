using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Appointments;
using Microsoft.EntityFrameworkCore;

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

        public async new Task<List<Appointment>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Appointments
                .OrderBy(x => x.Description);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Contact)
                .Include(p => p.AppointmentItems)
                .ToListAsync();

            return items;
        }

        public async Task<bool> ExistsAtDateTime(DateTimeOffset scheduledAt, long? excludeId = null)
        {
            return await context.Appointments.AnyAsync(a =>
                a.ScheduledAt == scheduledAt &&
                (!excludeId.HasValue || a.Id != excludeId.Value)
            );
        }

        public async new Task<Appointment?> GetByIdAsync(long id)
        {
            return await context.Appointments
                .Include(a => a.Contact)
                .Include(a => a.AppointmentItems)
                .ThenInclude(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}




