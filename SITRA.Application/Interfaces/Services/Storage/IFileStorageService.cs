namespace SITRA.Core.Interfaces.Services.Storage
{
    public interface IFileStorageService
    {
        Task DeleteAsync(string? url, string container);
        Task<string> SaveAsync(string container, IFormFile file);
        async Task<string> UpdateAsync(string? url, string container, IFormFile file)
        {
            await DeleteAsync(url, container);
            return await SaveAsync(container, file);
        }
    }
}

