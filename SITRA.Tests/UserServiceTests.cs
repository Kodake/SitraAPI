using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
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
using Xunit.Abstractions;
using SITRA.Infrastructure.Repositories;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Infrastructure.Services.Auths;

namespace SITRA.Tests
{
    public class UserServiceTests
    {
        private readonly ITestOutputHelper _output;

        public UserServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
            mockCurrentTenantService.Setup(x => x.TenantId).Returns((Guid?)Guid.NewGuid());

            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>())).ReturnsAsync(IdentityResult.Success);
            mockRoleManager.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(false);
            mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin")).ReturnsAsync(IdentityResult.Success);

            using (var context = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var mockUnitOfWork = new Mock<IUnitOfWork>();
                var mockRolePermissionRepo = new Mock<IRolePermissionRepository>();
                mockRolePermissionRepo.Setup(x => x.GetPermissionsByRoleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission>());
                mockUnitOfWork.Setup(x => x.RolePermissionRepository).Returns(mockRolePermissionRepo.Object);

                var mockLogger = new Mock<ILogger<UserService>>();
                var mockPermClaimLogger = new Mock<ILogger<PermissionClaimService>>();
                var mockAuthLogger = new Mock<ILogger<AuthenticationService>>();
                var mockRegLogger = new Mock<ILogger<UserRegistrationService>>();
                var registrationService = new UserRegistrationService(context, mockUserManager.Object, configuration, mockUnitOfWork.Object, new UserService(context, mockHttpContextAccessor.Object, mockUserManager.Object), mockRegLogger.Object);

                var dto = new CreateRegistrationUserDTO { Email = "admin@store.com", Password = "Abcd-1234", CompanyName = "Test Co", Plan = Core.Enums.PlanType.Free };

                // Act
                var result = await registrationService.RegisterAsync(dto, CancellationToken.None);

                // Assert
                Assert.True(result.IdentityResult.Succeeded);
                
                // Verify Tenant and Role were added to context
                Assert.True(await context.Tenants.IgnoreQueryFilters().AnyAsync(t => t.Email == "admin@store.com"));
                var adminRole = await context.TenantRoles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Name == "ADMIN");
                Assert.NotNull(adminRole);
                
                // Verify Permissions were linked
                var permissionsCount = await context.TenantRolePermissions.IgnoreQueryFilters().CountAsync(rp => rp.RoleId == adminRole.Id);
                Assert.True(permissionsCount > 0);
            }
        }

        [Fact]
        public async Task LoginAsync_ShouldPersistClaimsOnFirstLogin()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PersistClaimsTestDb")
                .Options;

            var tenantId = Guid.NewGuid();
            var user = new User { Id = "user-1", Email = "persist@test.com", TenantId = tenantId, UserName = "persist" };

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockCurrentTenantService = new Mock<ICurrentTenantService>();
            mockCurrentTenantService.Setup(x => x.TenantId).Returns((Guid?)tenantId);

            var myConfiguration = new Dictionary<string, string?>
            {
                {"Jwt:Audience", "test-audience"},
                {"Authentication:Schemes:Bearer:Issuer", "test-issuer"},
                {"Jwt:Key", "ThisIsATestKeyThatIsLongEnoughToBeAKey!!!"},
                {"ConnectionStrings:AzureStorage", "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net"}
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(myConfiguration).Build();

            using (var context = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var role = new Role { Id = Guid.NewGuid(), Name = "Manager", TenantId = tenantId, Description = "Manager" };
                var permission = new Permission { Id = Guid.NewGuid(), Key = "orders:write", Name = "Write Orders", Section = "Orders" };
                var rolePermission = new RolePermission { RoleId = role.Id, PermissionId = permission.Id };

                context.TenantRoles.Add(role);
                context.TenantPermissions.Add(permission);
                context.TenantRolePermissions.Add(rolePermission);
                
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null!, null!, null!, null!);
                var userStore = new UserStore<User>(context);
                var userManager = new UserManager<User>(userStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
                
                await userManager.CreateAsync(user, "Password1!");
                
                // We add the role to ASP.NET Identity system for the user
                // The UserManager needs to know about roles to return them in GetRolesAsync
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id.ToString() });
                // Also add to TenantRoleUsers which is what AuthenticationService queries
                context.TenantRoleUsers.Add(new TenantRoleUser { RoleId = role.Id, UserId = user.Id });
                
                await context.SaveChangesAsync();
            }

            // Act
            using (var testContext = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var roleStore = new RoleStore<IdentityRole>(testContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null!, null!, null!, null!);
                var userStore = new UserStore<User>(testContext);
                var userManager = new UserManager<User>(userStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
                
                // Create UnitOfWork which will create its own UserService
                var mockLogger = new Mock<ILogger<UserService>>();
                var mockPermClaimLogger = new Mock<ILogger<PermissionClaimService>>();
                var mockAuthLogger = new Mock<ILogger<AuthenticationService>>();
                var mockRegLogger = new Mock<ILogger<UserRegistrationService>>();
                var mockRoleServiceLogger = new Mock<ILogger<UserRoleService>>();
                var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                services.AddSingleton(new Mock<IInvoiceToEcfMapper>().Object);
                services.AddSingleton(new Mock<IEcfXmlGenerator>().Object);
                services.AddSingleton(new Mock<ISignatureService>().Object);
                services.AddSingleton(new Mock<IDgiiApiService>().Object);
                services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new SITRA.Core.Models.Dgii.DgiiSettings()));
                services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Messaging.IEmailService>().Object);
                services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService>().Object);
                services.AddSingleton(new Mock<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>().Object);
                var serviceProvider = services.BuildServiceProvider();

                var unitOfWork = new UnitOfWork(testContext, mockHttpContextAccessor.Object, userManager, roleManager, configuration, mockLogger.Object, mockPermClaimLogger.Object, mockAuthLogger.Object, mockRegLogger.Object, mockRoleServiceLogger.Object, serviceProvider);
                var authService = unitOfWork.AuthenticationService;
                
                var dto = new CreateLoginUserDTO { Email = "persist@test.com", Password = "Password1!" };
                var result = await authService.LoginAsync(dto, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.IsAuthenticated);

                // Verify claims are now in the database
                var persistedClaims = await testContext.UserClaims
                    .Where(c => c.UserId == user.Id && c.ClaimType == "Permission")
                    .ToListAsync();
                    
                Assert.Contains(persistedClaims, c => c.ClaimValue == "orders:write");
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
            mockCurrentTenantService.Setup(x => x.TenantId).Returns((Guid?)tenantId);

            var context = new ApplicationDbContext(options, mockCurrentTenantService.Object);
            var setupUserStore = new UserStore<User>(context);
            var setupUserManager = new UserManager<User>(setupUserStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
            
            using (context)
            {
                var permission = new Permission { Id = Guid.NewGuid(), Key = "products:read", Name = "Read Products", Section = "Products" };
                var role = new Role { Id = Guid.NewGuid(), Name = "Admin", TenantId = tenantId, Description = "Admin" };
                var rolePermission = new RolePermission { RoleId = role.Id, PermissionId = permission.Id };

                context.TenantPermissions.Add(permission);
                context.TenantRoles.Add(role);
                context.TenantRolePermissions.Add(rolePermission);
                
                await setupUserManager.CreateAsync(user, "Password1!");
                // Add to TenantRoleUsers which is what AuthenticationService queries
                context.TenantRoleUsers.Add(new TenantRoleUser { RoleId = role.Id, UserId = user.Id });
                // TODO:
                // - [x] Implement logic in `UserService.LoginAsync` <!-- id: 3 -->
                //    - [x] Check if user has claims in `AspNetUserClaims`
                //    - [x] If not, fetch role permissions from `TenantRolePermissions`
                //    - [x] Persist permissions as claims using `UserManager.AddClaimsAsync`
                //    - [x] Add all relevant claims to the JWT
                // - [x] Verify the changes <!-- id: 4 -->
                await context.SaveChangesAsync();
            }

            mockConfiguration.Setup(x => x[It.Is<string>(s => s.Contains("Jwt:Audience"))]).Returns("test-audience");
            mockConfiguration.Setup(x => x[It.Is<string>(s => s.Contains("Authentication:Schemes:Bearer:Issuer"))]).Returns("test-issuer");
            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("ThisIsATestKeyThatIsLongEnoughToBeAKey!!!");
            
            // Mock signing keys
            var mockKeysSection = new Mock<IConfigurationSection>();
            mockKeysSection.Setup(x => x.GetChildren()).Returns(new List<IConfigurationSection>().AsEnumerable());
            mockConfiguration.Setup(x => x.GetSection("Authentication:Schemes:Bearer:SigningKeys")).Returns(mockKeysSection.Object);
            
            var mockConnSection = new Mock<IConfigurationSection>();
            mockConnSection.Setup(x => x["AzureStorage"]).Returns("DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net");
            mockConfiguration.Setup(x => x.GetSection("ConnectionStrings")).Returns(mockConnSection.Object);

            using (var testContext = new ApplicationDbContext(options, mockCurrentTenantService.Object))
            {
                var roleStore = new RoleStore<IdentityRole>(testContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null!, null!, null!, null!);
                var userStore = new UserStore<User>(testContext);
                var userManager = new UserManager<User>(userStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
                
                var mockLogger = new Mock<ILogger<UserService>>();
                var mockPermClaimLogger = new Mock<ILogger<PermissionClaimService>>();
                var mockAuthLogger = new Mock<ILogger<AuthenticationService>>();
                var mockRegLogger = new Mock<ILogger<UserRegistrationService>>();
                var mockRoleServiceLogger = new Mock<ILogger<UserRoleService>>();
                var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                services.AddSingleton(new Mock<IInvoiceToEcfMapper>().Object);
                services.AddSingleton(new Mock<IEcfXmlGenerator>().Object);
                services.AddSingleton(new Mock<ISignatureService>().Object);
                services.AddSingleton(new Mock<IDgiiApiService>().Object);
                services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new SITRA.Core.Models.Dgii.DgiiSettings()));
                services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Messaging.IEmailService>().Object);
                services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService>().Object);
                services.AddSingleton(new Mock<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>().Object);
                var serviceProvider = services.BuildServiceProvider();

                var unitOfWork = new UnitOfWork(testContext, mockHttpContextAccessor.Object, userManager, roleManager, mockConfiguration.Object, mockLogger.Object, mockPermClaimLogger.Object, mockAuthLogger.Object, mockRegLogger.Object, mockRoleServiceLogger.Object, serviceProvider);
                var authService = unitOfWork.AuthenticationService;
                
                var dto = new CreateLoginUserDTO { Email = "test@test.com", Password = "Password1!" };
                var result = await authService.LoginAsync(dto, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.IsAuthenticated);
                Assert.NotNull(result.Token);
            }
        }

        [Fact]
        public async Task TestRealDbUpdateProfile()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=.;Database=SITRA;Integrated Security=True;TrustServerCertificate=True")
                .Options;

            var mockTenantService = new Mock<ICurrentTenantService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            
            // Log in as sitra@gmail.com: we need their UserId
            using (var context = new ApplicationDbContext(options, mockTenantService.Object))
            {
                var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == "SITRA@GMAIL.COM");
                if (user == null) throw new Exception("User not found");

                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) };
                var identity = new ClaimsIdentity(claims, "Test");
                httpContext.User = new ClaimsPrincipal(identity);
            }

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLocalization();
            
            // Add DataProtection
            services.AddDataProtection();
            
            // Mock dependency services
            services.AddSingleton(new Mock<IInvoiceToEcfMapper>().Object);
            services.AddSingleton(new Mock<IEcfXmlGenerator>().Object);
            services.AddSingleton(new Mock<ISignatureService>().Object);
            services.AddSingleton(new Mock<IDgiiApiService>().Object);
            services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new SITRA.Core.Models.Dgii.DgiiSettings { UseAzureStorage = true }));
            services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Messaging.IEmailService>().Object);
            services.AddSingleton(new Mock<SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService>().Object);

            var serviceProvider = services.BuildServiceProvider();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Audience", "test-audience"},
                    {"Authentication:Schemes:Bearer:Issuer", "test-issuer"},
                    {"Jwt:Key", "ThisIsATestKeyThatIsLongEnoughToBeAKey!!!"},
                    {"ConnectionStrings:AzureStorage", "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net"}
                })
                .Build();

            using (var context = new ApplicationDbContext(options, mockTenantService.Object))
            {
                var userStore = new UserStore<User>(context);
                var userManager = new UserManager<User>(userStore, null!, new PasswordHasher<User>(), null!, null!, null!, null!, null!, null!);
                
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null!, null!, null!, null!);

                var unitOfWork = new UnitOfWork(
                    context, 
                    mockHttpContextAccessor.Object, 
                    userManager, 
                    roleManager, 
                    configuration, 
                    new Mock<ILogger<UserService>>().Object, 
                    new Mock<ILogger<PermissionClaimService>>().Object, 
                    new Mock<ILogger<AuthenticationService>>().Object, 
                    new Mock<ILogger<UserRegistrationService>>().Object, 
                    new Mock<ILogger<UserRoleService>>().Object, 
                    serviceProvider);

                var dto = new UpdateProfileNameDTO
                {
                    UserName = "SITRA",
                    Address = "13 DE SEPTIEMBRE",
                    Province = "010000",
                    Municipality = "010101",
                    PhoneNumber = "8091414151"
                };

                try
                {
                    _output.WriteLine("Calling UpdateProfileNameAsync...");
                    var result = await unitOfWork.UserRoleService.UpdateProfileNameAsync(dto, CancellationToken.None);
                    _output.WriteLine("Success: " + (result != null));
                }
                catch (Exception ex)
                {
                    _output.WriteLine("EXCEPTION THROWS:");
                    _output.WriteLine(ex.ToString());
                    if (ex.InnerException != null)
                    {
                        _output.WriteLine("INNER EXCEPTION:");
                        _output.WriteLine(ex.InnerException.ToString());
                    }
                    throw;
                }
            }
        }
    }
}
