using MediatR;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using SITRA.Core.DTOs.AdministrativeTools;

namespace SITRA.Core.Mediator.Handlers.AdministrativeTools.Roles
{
    public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AssignPermissionsCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<bool> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the custom role in the business repository
            var customRole = await _unitOfWork.RoleRepository.GetByIdAsync(request.DTO.RoleId, cancellationToken);
            if (customRole == null) return false;

            // 2. Load Permission Map and Sync Permissions Table from configuration
            // This ensures that human-readable keys (like "categories:read") sent by the frontend exist in the DB.
            var config = _configuration.GetSection("PermissionManagement").Get<PermissionManagementConfigDTO>();
            if (config != null)
            {
                var existingPermissions = await _unitOfWork.PermissionRepository.GetAllAsync(p => true, cancellationToken);
                var existingPermissionKeys = existingPermissions.ToDictionary(p => p.Key, p => p.Id);

                foreach (var module in config.Modules)
                {
                    foreach (var action in module.Actions)
                    {
                        var key = $"{module.Key}:{action.Name}";
                        if (!existingPermissionKeys.ContainsKey(key))
                        {
                            // Defensive: Ensure ID is also unique to avoid PK violation if config is messy
                            // Check both existing DB entries AND newly added ones in this transaction
                            var existingById = existingPermissions.FirstOrDefault(p => p.Id == action.Id);
                            if (existingById != null)
                            {
                                existingPermissionKeys[key] = existingById.Id;
                                continue;
                            }

                            var newPermission = new Core.Entities.AdministrativeTools.Permission
                            {
                                Id = action.Id,
                                Name = action.Name,
                                Key = key,
                                Section = module.Section
                            };
                            await _unitOfWork.PermissionRepository.CreateAsync(newPermission, cancellationToken);
                            existingPermissionKeys[key] = newPermission.Id;
                            existingPermissions.Add(newPermission); // Add to local list to track duplicates in config
                        }
                    }
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // 3. Resolve Permission Identifiers to GUID IDs
            var resolvedGuidIds = new List<Guid>();
            var allStoredPermissions = await _unitOfWork.PermissionRepository.GetAllAsync(p => true, cancellationToken);
            var keyToIdMap = allStoredPermissions.GroupBy(p => p.Key).ToDictionary(g => g.Key, g => g.First().Id);
            var idToIdMap = allStoredPermissions.GroupBy(p => p.Id).ToDictionary(g => g.Key.ToString().ToLower(), g => g.First().Id);

            foreach (var identifier in request.DTO.PermissionIds)
            {
                if (string.IsNullOrWhiteSpace(identifier)) continue;

                var lowerIdentifier = identifier.ToLower().Trim();

                // Option A: It's a GUID string (matches an ID)
                if (idToIdMap.TryGetValue(lowerIdentifier, out var idByMatch))
                {
                    resolvedGuidIds.Add(idByMatch);
                }
                // Option B: It's a Key string (matches a Key)
                else if (keyToIdMap.TryGetValue(identifier, out var idByKey)) // Use original case for keys if needed, or lower depending on storage
                {
                    resolvedGuidIds.Add(idByKey);
                }
                // Option C: It's a GUID string that isn't in our map yet but is valid GUID
                else if (Guid.TryParse(identifier, out var parsedGuid))
                {
                    resolvedGuidIds.Add(parsedGuid);
                }
            }

            // 4. Update Custom Role Permissions Table
            await _unitOfWork.RolePermissionRepository.UpdateRolePermissionsAsync(request.DTO.RoleId, resolvedGuidIds.Distinct().ToList(), cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
