using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.DTOs.Reporting
{
    public class ReportDTO
    {
        public ReportDTO(List<Invoice> invoices, DateTime from, DateTime to)
        {
            Invoices = invoices;
            From = from;
            To = to;
        }

        public List<Invoice> Invoices { get; }
        public DateTime From { get; }
        public DateTime To { get; }
    }
}
