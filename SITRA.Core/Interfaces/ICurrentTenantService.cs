namespace SITRA.Core.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid? TenantId { get; }
    }
}
