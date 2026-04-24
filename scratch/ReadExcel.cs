using System;
using System.IO;
using ClosedXML.Excel;
using System.Linq;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"c:\Users\Kodake\Desktop\SITRA SRC\PosAPI\SITRA.Core\Resources\133556545-22042026235741.xlsx";
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found: " + path);
                return;
            }

            using (var workbook = new XLWorkbook(path))
            {
                foreach (var sheet in workbook.Worksheets)
                {
                    Console.WriteLine($"--- Sheet: {sheet.Name} ---");
                    var range = sheet.RangeUsed();
                    if (range == null) continue;
                    
                    var rows = range.RowsUsed().Take(10); // First 10 rows
                    foreach (var row in rows)
                    {
                        var cells = row.CellsUsed();
                        Console.WriteLine(string.Join(" | ", cells.Select(c => c.Value.ToString())));
                    }
                }
            }
        }
    }
}
