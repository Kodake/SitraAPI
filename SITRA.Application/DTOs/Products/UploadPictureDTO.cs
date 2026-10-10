using Microsoft.AspNetCore.Http;

namespace SITRA.Core.DTOs.Products
{
    public class UploadPictureDTO
    {
        public IFormFile? File { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? ImageUrl { get; set; }

        public IFormFile? GetFile() => File ?? Image ?? ImageUrl;
    }
}
