using System.IO;
using System.Collections.Generic;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Core.Interfaces
{
    public interface ITestCasesReader
    {
        List<EcfDocument> ReadTestCases(Stream stream);
    }
}
