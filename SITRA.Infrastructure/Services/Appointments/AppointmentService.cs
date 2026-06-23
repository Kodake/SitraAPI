using SITRA.Core.Entities.Appointments;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Appointments;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
namespace SITRA.Infrastructure.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SITRA.Infrastructure.Persistence.ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;

        public AppointmentService(IUnitOfWork unitOfWork, SITRA.Infrastructure.Persistence.ApplicationDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Appointment>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Appointment, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.AppointmentRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<List<Appointment>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Appointments.AsNoTracking();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Description != null && x.Description.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(x => x.Description);

            var httpContext = _httpContext;

            if (httpContext != null)
            {
                await SITRA.Core.Helpers.HttpContextExtensions.AddPaginationParamsToHeaders(httpContext, queryable);
            }

            var items = await SITRA.Core.Helpers.IQueryableExtensions.Paginate(queryable, paginationDTO)
                .Include(p => p.Contact)
                .Include(p => p.AppointmentItems)
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
