namespace SITRA.Core.DTOs.AdministrativeTools.Roles
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
        public List<ActionConfigDTO> Actions { get; set; } = new();
    }

    public class ActionConfigDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
