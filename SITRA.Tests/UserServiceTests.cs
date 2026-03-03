using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Services;
using System.Security.Claims;
using Xunit;

namespace SITRA.Tests
{
    public class UserServiceTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null!, null!, null!, null!);
        }

        [Fact]
        public async Task RegisterAssociatedUserAsync_ValidRequest_CreatesUserAndAssignsRoleAndClaims()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();
            var mockRoleManager = GetMockRoleManager();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockConfiguration = new Mock<IConfiguration>();

            var tenantId = Guid.NewGuid();
            var parentUserId = Guid.NewGuid().ToString();
            var roleId = Guid.NewGuid();
            var permissionClaim = new Claim("Permission", "products:write");

            // Setup HttpContext to return the currently logged-in parent user's Email
            var userEmail = "plus@gmail.com";
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, userEmail) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var defaultHttpContext = new DefaultHttpContext { User = claimsPrincipal };
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(defaultHttpContext);

            // Mock GetUserAsync behavior inside RegisterAssociatedUserAsync
            var parentUser = new User
            {
                Id = parentUserId,
                Email = userEmail,
                TenantId = tenantId
            };
            mockUserManager.Setup(x => x.FindByEmailAsync(userEmail)).ReturnsAsync(parentUser);

            // Mock CreateAsync for the sub user
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), "Abcd-1234"))
                           .ReturnsAsync(IdentityResult.Success);

            // Mock RoleManager lookups
            var identityRole = new IdentityRole("Cashier") { Id = roleId.ToString() };
            mockRoleManager.Setup(x => x.FindByIdAsync(roleId.ToString())).ReturnsAsync(identityRole);

            // Mock RoleManager returning the claims for this role
            mockRoleManager.Setup(x => x.GetClaimsAsync(identityRole))
                           .ReturnsAsync(new List<Claim> { permissionClaim });

            // Setup AddToRole
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Cashier"))
                           .ReturnsAsync(IdentityResult.Success);

            // Setup AddClaimAsync
            mockUserManager.Setup(x => x.AddClaimAsync(It.IsAny<User>(), It.IsAny<Claim>()))
                           .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                null!, // ApplicationDbContext not used during this specific flow
                mockHttpContextAccessor.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockConfiguration.Object
            );

            var dto = new CreateAssociatedUserDTO
            {
                Email = "subuser@store.com",
                Password = "Abcd-1234",
                RoleId = roleId
            };

            // Act
            var result = await userService.RegisterAssociatedUserAsync(dto, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);

            // 1. Verify User was created with exact password "Abcd-1234", inherited TenantId, and ParentUserId
            mockUserManager.Verify(x => x.CreateAsync(It.Is<User>(u => 
                u.Email == "subuser@store.com" && 
                u.TenantId == tenantId && 
                u.ParentUserId == parentUserId), "Abcd-1234"), Times.Once);

            // 2. Verify subUser was assigned the role "Cashier"
            mockUserManager.Verify(x => x.AddToRoleAsync(It.Is<User>(u => u.Email == "subuser@store.com"), "Cashier"), Times.Once);

            // 3. Verify exact permission claims ("products:write") were copied to the user directly
            mockUserManager.Verify(x => x.AddClaimAsync(It.Is<User>(u => u.Email == "subuser@store.com"), 
                It.Is<Claim>(c => c.Type == "Permission" && c.Value == "products:write")), Times.Once);
        }
    }
}
