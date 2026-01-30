using Microsoft.EntityFrameworkCore;

namespace SITRA.Core.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task AddPaginationParamsToHeaders<T>(
            this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }

            double totalRecords = await queryable.CountAsync();
            httpContext.Response.Headers.Append("totalCountRecords", totalRecords.ToString());
        }
    }
}

