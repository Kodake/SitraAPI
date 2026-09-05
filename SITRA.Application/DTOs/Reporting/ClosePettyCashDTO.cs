using SITRA.Core.DTOs.CashBoxes;

namespace SITRA.Core.DTOs.Reporting
{
    public class ClosePettyCashDTO
    {
        public ClosePettyCashDTO(PettyCashDTO pettyCash)
        {
            PettyCash = pettyCash;
        }

        public PettyCashDTO PettyCash { get; }
    }
}
