using BackEnd.Entities.AdministrativeTools;

namespace BackEnd.Services.AdministrativeTools
{
    public interface IFiscalSupportService
    {
        string GenerateFiscalSupportNumber(FiscalSupport fiscalSupport);
    }
}
