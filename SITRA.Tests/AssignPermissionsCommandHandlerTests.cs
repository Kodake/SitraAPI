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
        public async Task Handle_ValidRequest_AssignsPermissionsToRepository()
        {
            // Arrange
            var mockRoleManager = GetMockRoleManager();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRoleRepo = new Mock<IRoleRepository>();
            var mockPermissionRepo = new Mock<IPermissionRepository>();
            var mockRolePermissionRepo = new Mock<IRolePermissionRepository>();

            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var permissionKey = "products:read";

            var customRole = new Role { Id = roleId, Name = "Manager Role", TenantId = Guid.NewGuid() };
            var permission = new Permission { Id = permissionId, Key = permissionKey };

            var dto = new AssignPermissionsDTO
            {
                RoleId = roleId,
                PermissionIds = new List<string> { permissionKey } // Passing the KEY
            };

            var command = new AssignPermissionsCommand(dto);

            // Mock IUnitOfWork repository
            mockRoleRepo.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(customRole);
            
            mockPermissionRepo.Setup(p => p.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Permission, bool>>>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new List<Permission> { permission });

            mockUnitOfWork.Setup(u => u.RoleRepository).Returns(mockRoleRepo.Object);
            mockUnitOfWork.Setup(u => u.PermissionRepository).Returns(mockPermissionRepo.Object);
            mockUnitOfWork.Setup(u => u.RolePermissionRepository).Returns(mockRolePermissionRepo.Object);

            // Mock Configuration
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
            
            // Verify UpdateRolePermissionsAsync was called with the RESOLVED GUID
            mockRolePermissionRepo.Verify(r => r.UpdateRolePermissionsAsync(
                roleId, 
                It.Is<List<Guid>>(list => list.Contains(permissionId)), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        }
    }
}
