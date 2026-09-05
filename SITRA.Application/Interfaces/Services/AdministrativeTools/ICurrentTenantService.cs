namespace SITRA.Core.Interfaces.Services.AdministrativeTools
{
    public interface ICurrentTenantService
    {
        Guid? TenantId { get; }
    }
}
