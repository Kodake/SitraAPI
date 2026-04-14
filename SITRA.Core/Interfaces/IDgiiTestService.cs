using System.Collections.Generic;
using System.Threading.Tasks;

namespace SITRA.Core.Interfaces
{
    public interface IDgiiTestService
    {
        Task<DgiiTestSuiteResult> RunTestsAsync(string? ecfType = null);
    }

    public class DgiiTestSuiteResult
    {
        public string? Token { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<DgiiTestResult> Results { get; set; } = new();
    }

    public class DgiiTestResult
    {
        public string TestName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TrackId { get; set; }
        public string? Error { get; set; }
    }
}
