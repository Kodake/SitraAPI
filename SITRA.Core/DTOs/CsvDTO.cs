using Microsoft.AspNetCore.Http;
namespace SITRA.Core.DTOs
{
    public class CsvDTO
    {
        public required IFormFile CsvFile { get; set; }
    }
}

