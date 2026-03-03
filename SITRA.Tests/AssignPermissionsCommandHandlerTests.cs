using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SITRA.Core.DTOs.AdministrativeTools;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Handlers.AdministrativeTools.Roles;
using System.Security.Claims;
using Xunit;

namespace SITRA.Tests
{
    public class AssignPermissionsCommandHandlerTests
    {
        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null!, null!, null!, null!);
        }

        [Fact]
        public async Task Handle_ValidRequest_AssignsClaimsCorrectly()
        {
            // Arrange
            var mockRoleManager = GetMockRoleManager();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRoleRepo = new Mock<IRoleRepository>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            var mockConfiguration = new Mock<IConfiguration>();

            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid(); // Random permission action id

            var customRole = new Role { Id = roleId, Name = "Manager Role", TenantId = Guid.NewGuid() };
            var identityRole = new IdentityRole("Manager Role");

            var dto = new AssignPermissionsDTO
            {
                RoleId = roleId,
                PermissionIds = new List<Guid> { permissionId }
            };

            var command = new AssignPermissionsCommand(dto);

            // Mock IUnitOfWork repository
            mockRoleRepo.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(customRole);
            mockUnitOfWork.Setup(u => u.RoleRepository).Returns(mockRoleRepo.Object);

            // Mock Identity Role manager lookups
            mockRoleManager.Setup(r => r.FindByNameAsync("Manager Role"))
                           .ReturnsAsync(identityRole);

            // Mock Identity claims responses
            mockRoleManager.Setup(r => r.GetClaimsAsync(identityRole))
                           .ReturnsAsync(new List<Claim>());

            mockRoleManager.Setup(r => r.AddClaimAsync(identityRole, It.IsAny<Claim>()))
                           .ReturnsAsync(IdentityResult.Success);

            // Mock Configuration for permission dict mapping
            var configData = new PermissionManagementConfigDTO
            {
                Modules = new List<ModuleConfigDTO>
                {
                    new ModuleConfigDTO
                    {
                        Key = "products",
                        Actions = new List<ActionConfigDTO>
                        {
                            new ActionConfigDTO { Id = permissionId, Name = "read" }
                        }
                    }
                }
            };
            
            // Note: Since .Get<T>() is an extension method, we usually have to mock the config root 
            // but for simplicity in IConfiguration returning an object directly we can build a stub if needed
            // However, Moq struggles with Get<T>() extension cleanly unless we wrap it or use IOptions.
            // But we will just try to bypass/create a concrete Config using InMemory configuration safely.

            var inMemorySettings = new Dictionary<string, string?> {
                {"PermissionManagement:Modules:0:Key", "products"},
                {"PermissionManagement:Modules:0:Actions:0:Id", permissionId.ToString()},
                {"PermissionManagement:Modules:0:Actions:0:Name", "read"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var handler = new AssignPermissionsCommandHandler(
                mockRoleManager.Object, 
                mockUnitOfWork.Object, 
                configuration);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            
            // Verify AddClaimAsync was called perfectly with "products:read" !
            mockRoleManager.Verify(r => r.AddClaimAsync(
                identityRole, 
                It.Is<Claim>(c => c.Type == "Permission" && c.Value == "products:read")), 
            Times.Once);
        }
    }
}
