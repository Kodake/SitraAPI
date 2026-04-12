using System.Collections.Generic;
using System.Threading.Tasks;

namespace SITRA.Core.Interfaces
{
    public interface IDgiiTestService
    {
        Task<List<DgiiTestResult>> RunTestsAsync(string? ecfType = null);
    }

    public class DgiiTestResult
    {
        public string TestName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TrackId { get; set; }
        public string? Error { get; set; }
    }
}
