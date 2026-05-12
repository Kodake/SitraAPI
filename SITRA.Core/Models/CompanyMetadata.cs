namespace SITRA.Core.Models
{
    public class CompanyMetadata
    {
        public string? CompanyName { get; set; }
        public string? RucNumber { get; set; }
        public string? Address { get; set; }
        public string? ProvinceCode { get; set; }
        public string? MunicipalityCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        
        // Computed resolved display values
        public string? ResolvedProvince { get; set; }
        public string? ResolvedMunicipality { get; set; }
    }
}
