namespace SITRA.Core.DTOs.AdministrativeTools
{
    public class PermissionManagementConfigDTO
    {
        public List<ModuleConfigDTO> Modules { get; set; } = new();
    }

    public class ModuleConfigDTO
    {
        public string Name { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Section { get; set; } = null!;
        public List<string> Actions { get; set; } = new();
    }
}
