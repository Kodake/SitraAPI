using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Services;
using System.Security.Claims;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        private Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        // Helper classes for Async support
        private class TestAsyncQueryProvider<TEntity> : Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
            public IQueryable CreateQuery(System.Linq.Expressions.Expression expression) => new TestAsyncEnumerable<TEntity>(new EFExpressionVisitor().Visit(expression));
            public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression) => new TestAsyncEnumerable<TElement>(new EFExpressionVisitor().Visit(expression));
            public object Execute(System.Linq.Expressions.Expression expression) => _inner.Execute(new EFExpressionVisitor().Visit(expression))!;
            public TResult Execute<TResult>(System.Linq.Expressions.Expression expression) => _inner.Execute<TResult>(new EFExpressionVisitor().Visit(expression));
            public TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default) => Execute<TResult>(expression);
        }

        private class EFExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression node)
            {
                if (node.Method.Name == "IgnoreQueryFilters")
                {
                    return Visit(node.Arguments[0]);
                }
                return base.VisitMethodCall(node);
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(System.Linq.Expressions.Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
            public T Current => _inner.Current;
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
            public ValueTask DisposeAsync() { _inner.Dispose(); return new ValueTask(); }
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_CreatesAdminRoleAndSyncsPermissions()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RegisterTestDb")
                .Options;

            var mockUserManager = GetMockUserManager();
            var mockRoleManager = GetMockRoleManager();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var myConfiguration = new Dictionary<string, string?>
            {
                {"PermissionManagement:Modules:0:Name", "Dashboards"},
                {"PermissionManagement:Modules:0:Key", "dashboards"},
                {"PermissionManagement:Modules:0:Section", "admin"},
                {"PermissionManagement:Modules:0:Actions:0:Id", Guid.NewGuid().ToString()},
                {"PermissionManagement:Modules:0:Actions:0:Name", "read"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var mockCurrentTenantService = new Mock<ICurrentTenantService>();
            mockCurrentTenantService.Setup(x => x.TenantId).Returns(Guid.NewGuid());

            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(false);
            mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin")).ReturnsAsync(IdentityResult.Success);

            using (var context = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var userService = new UserService(context, mockHttpContextAccessor.Object, mockUserManager.Object, configuration);

                var dto = new CreateRegistrationUserDTO { Email = "admin@store.com", Password = "Abcd-1234", CompanyName = "Test Co", Plan = Core.Enums.PlanType.Free };

                // Act
                var result = await userService.RegisterAsync(dto, CancellationToken.None);

                // Assert
                Assert.True(result.Succeeded);
                mockRoleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Admin")), Times.Once);
                mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin"), Times.Once);
                
                // Verify Tenant and Role were added to context
                Assert.True(await context.Tenants.AnyAsync(t => t.Email == "qa@gmail.com"));
                var adminRole = await context.TenantRoles.FirstOrDefaultAsync(r => r.Name == "Admin");
                Assert.NotNull(adminRole);
                
                // Verify Permissions were linked
                var permissionsCount = await context.TenantRolePermissions.CountAsync(rp => rp.RoleId == adminRole.Id);
                Assert.True(permissionsCount > 0);
            }
        }

        [Fact]
        public async Task LoginAsync_ValidRequest_FetchesPermissionsFromCustomTable()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "LoginTestDb")
                .Options;

            var tenantId = Guid.NewGuid();
            var user = new User { Id = "1", Email = "test@test.com", TenantId = tenantId, UserName = "test" };

            var mockUserManager = GetMockUserManager();
            var mockRoleManager = GetMockRoleManager();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockConfiguration = new Mock<IConfiguration>();

            var mockCurrentTenantService = new Mock<ICurrentTenantService>();
            mockCurrentTenantService.Setup(x => x.TenantId).Returns(tenantId);

            var context = new ApplicationDbContext(options, mockCurrentTenantService.Object);
            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
            
            using (context)
            {
                var permission = new Permission { Id = Guid.NewGuid(), Key = "products:read", Name = "Read Products", Section = "Products" };
                var role = new Role { Id = Guid.NewGuid(), Name = "Admin", TenantId = tenantId, Description = "Admin" };
                var rolePermission = new RolePermission { RoleId = role.Id, PermissionId = permission.Id };

                context.TenantPermissions.Add(permission);
                context.TenantRoles.Add(role);
                context.TenantRolePermissions.Add(rolePermission);
                
                await userManager.CreateAsync(user, "Password1!");
                await context.SaveChangesAsync();
            }

            mockConfiguration.Setup(x => x[It.Is<string>(s => s.Contains("Jwt:Audience"))]).Returns("test-audience");
            mockConfiguration.Setup(x => x[It.Is<string>(s => s.Contains("Authentication:Schemes:Bearer:Issuer"))]).Returns("test-issuer");
            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("ThisIsATestKeyThatIsLongEnoughToBeAKey!!!");
            
            // Mock signing keys
            var mockKeysSection = new Mock<IConfigurationSection>();
            mockKeysSection.Setup(x => x.GetChildren()).Returns(new List<IConfigurationSection>().AsEnumerable());
            mockConfiguration.Setup(x => x.GetSection("Authentication:Schemes:Bearer:SigningKeys")).Returns(mockKeysSection.Object);

            using (var testContext = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var realUserManager = new UserManager<User>(new UserStore<User>(testContext), null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
                var userService = new UserService(testContext, mockHttpContextAccessor.Object, realUserManager, mockConfiguration.Object);
                var dto = new CreateLoginUserDTO { Email = "test@test.com", Password = "Password1!" };

                // Act
                var result = await userService.LoginAsync(dto, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.IsAuthenticated);
            }
        }
    }
}
