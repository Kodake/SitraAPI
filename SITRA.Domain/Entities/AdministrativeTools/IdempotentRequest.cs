namespace SITRA.Core.Entities.AdministrativeTools
{
    public class IdempotentRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int ResponseStatusCode { get; set; }
        public string? ResponseBody { get; set; }
    }
}
