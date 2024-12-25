using BackEnd.Helpers;

namespace BackEnd.DTOs
{
    public class PaginationDTO
    {
        private const int initialPage = 1;
        private const int initialRecordsPerPage = 10;
        public int Page { get; set; } = initialPage;
        private int recordsPerPage = initialRecordsPerPage;
        private readonly int maxRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }

            set
            {
                recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value;
            }
        }

        public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
        {
            var page = context.ExtractDefaultValue(nameof(Page), initialPage);
            var recordsPerPage = context.ExtractDefaultValue(nameof(RecordsPerPage), initialRecordsPerPage);

            var resultado = new PaginationDTO
            {
                Page = page,
                RecordsPerPage = recordsPerPage
            };

            return ValueTask.FromResult(resultado);
        }
    }
}
