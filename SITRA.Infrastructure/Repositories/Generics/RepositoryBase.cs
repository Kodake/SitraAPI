using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Core.DTOs;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Generics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    public async Task<List<T>> GetAllAsync(PaginationDTO paginationDTO)
    {
        var queryable = context.Set<T>().AsQueryable();

        if (httpContext != null)
        {
            await httpContext.AddPaginationParamsToHeaders(queryable);
        }

        return await queryable
            .OrderBy(x => EF.Property<Guid>(x, "Id"))
            .Paginate(paginationDTO)
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await context.Set<T>().Where(filter).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        EntityEntry<T> inserted = await context.Set<T>().AddAsync(entity);
        return inserted.Entity;
    }

    public T Update(T entity)
    {
        EntityEntry<T> updated = context.Set<T>().Update(entity);
        return updated.Entity;
    }

    public async Task ActivateAsync(Guid id)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            var property = typeof(T).GetProperty("IsActive");
            property?.SetValue(entity, true);
        }
    }

    public async Task DeactivateAsync(Guid id)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            var property = typeof(T).GetProperty("IsActive");
            property?.SetValue(entity, false);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Set<T>().AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
    }
}



}
