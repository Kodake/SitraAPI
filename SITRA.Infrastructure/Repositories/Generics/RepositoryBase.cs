using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SITRA.Core.DTOs;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Generics;
using SITRA.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Repositories.Generics
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly ApplicationDbContext context;
        protected readonly HttpContext? httpContext;

        public RepositoryBase(ApplicationDbContext context, IHttpContextAccessor? httpContextAccessor = null)
        {
            this.context = context;
            this.httpContext = httpContextAccessor?.HttpContext;
        }

        public async Task<List<T>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Set<T>().AsQueryable();

            if (httpContext != null)
            {
                await httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .OrderBy(x => EF.Property<Guid>(x, "Id"))
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().Where(filter).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<T> inserted = await context.Set<T>().AddAsync(entity, cancellationToken);
            return inserted.Entity;
        }

        public T Update(T entity)
        {
            EntityEntry<T> updated = context.Set<T>().Update(entity);
            return updated.Entity;
        }

        public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                var property = typeof(T).GetProperty("IsActive");
                property?.SetValue(entity, true);
            }
        }

        public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                var property = typeof(T).GetProperty("IsActive");
                property?.SetValue(entity, false);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().CountAsync(filter, cancellationToken);
        }
        public async Task<List<T>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Set<T>().AsNoTracking().AsQueryable();

            if (httpContext != null)
            {
                await httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .OrderBy(x => EF.Property<Guid>(x, "Id"))
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetReadOnlyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().AsNoTracking().Where(filter).ToListAsync(cancellationToken);
        }
    }
}
