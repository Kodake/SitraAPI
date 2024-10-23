using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.AdministrativeTools
{
    public class DeadlineRepository : IDeadlineRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public DeadlineRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Deadline>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Deadlines.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Deadline>> GetAll(StatusType status)
        {
            IQueryable<Deadline> query = context.Deadlines.AsQueryable();

            switch (status)
            {
                case StatusType.Activated:
                    query = query.Where(u => u.IsActive);
                    break;
                case StatusType.Deactivated:
                    query = query.Where(u => !u.IsActive);
                    break;
                case StatusType.All:
                default:
                    break;
            }

            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<Deadline?> GetById(int id)
        {
            return await context.Deadlines.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Deadline deadline)
        {
            context.Add(deadline);
            await context.SaveChangesAsync();
            return deadline.Id;
        }

        public async Task Update(Deadline deadline)
        {
            context.Update(deadline);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var deadline = await context.Deadlines.FirstOrDefaultAsync(x => x.Id == id);
            if (deadline != null)
            {
                deadline.IsActive = true;
                context.Entry(deadline).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var deadline = await context.Deadlines.FirstOrDefaultAsync(x => x.Id == id);
            if (deadline != null)
            {
                deadline.IsActive = false;
                context.Entry(deadline).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Deadlines.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, int days)
        {
            return await context.Deadlines.AnyAsync(x => x.Id != id && x.Days == days);
        }
    }
}
