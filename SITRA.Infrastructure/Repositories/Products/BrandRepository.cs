using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class BrandRepository : RepositoryBase<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }



        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Brands.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }
    }
}





