using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using SITRA.Infrastructure.Services.Auths;
using SITRA.Infrastructure.Services.AdministrativeTools;
using SITRA.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces;
using SITRA.Infrastructure.Persistence.Accounts;
using SITRA.Infrastructure.Persistence.AdministrativeTools;
using SITRA.Infrastructure.Persistence.Appointments;
using SITRA.Infrastructure.Persistence.CashBoxes;
using SITRA.Infrastructure.Persistence.Contacts;
using SITRA.Infrastructure.Persistence.Finances;
using SITRA.Infrastructure.Persistence.Invoices;
using SITRA.Infrastructure.Persistence.Products;
using SITRA.Infrastructure.Persistence.Warehouses;
using System.Reflection;

namespace SITRA.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly ICurrentTenantService _currentTenantService;

        public ApplicationDbContext(DbContextOptions options, ICurrentTenantService currentTenantService) : base(options)
        {
            _currentTenantService = currentTenantService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<ITenant>())
            {
                if (entry.State == EntityState.Added && 
                    _currentTenantService.TenantId.HasValue && 
                    entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = _currentTenantService.TenantId.Value;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Query Filter for Multi-Tenancy
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ITenant).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetGlobalQuery), BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
            modelBuilder.ApplyConfiguration(new PayableConfiguration());
            modelBuilder.ApplyConfiguration(new PayableReceiptConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentReceiptConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivableConfiguration());

            modelBuilder.ApplyConfiguration(new DeadlineConfiguration());
            modelBuilder.ApplyConfiguration(new FiscalSupportConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new TenantRoleUserConfiguration());

            modelBuilder.ApplyConfiguration(new ContactConfiguration());

            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTaxesConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());

            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceItemConfiguration());
            modelBuilder.ApplyConfiguration(new QuotationConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new SpotPaymentConfiguration());
            modelBuilder.ApplyConfiguration(new SpotPayableConfiguration());

            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new ModelConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new ComboItemConfiguration());
            modelBuilder.ApplyConfiguration(new PricingConfiguration());
            modelBuilder.ApplyConfiguration(new PricingItemConfiguration());

            modelBuilder.ApplyConfiguration(new StockConfiguration());
            modelBuilder.ApplyConfiguration(new TransferConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());

            modelBuilder.ApplyConfiguration(new PettyCashConfiguration());
            modelBuilder.ApplyConfiguration(new PettyCashMovementConfiguration());

            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentItemConfiguration());

            // ===============================
            // Identity ? schema auths
            // ===============================
            modelBuilder.Entity<User>().ToTable("AspNetUsers", "auths");
            // Role configuration is handled by RoleConfiguration class
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "auths");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "auths");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "auths");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "auths");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "auths");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "auths");
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens", "auths").HasKey(t => t.Id);

            // ===============================
            // Audits ? schema audits
            // ===============================
            modelBuilder.Entity<IdempotentRequest>().ToTable("IdempotentRequests", "audits");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs", "audits");

            modelBuilder.Entity<User>()
                .Property(u => u.Language)
                .HasConversion<string>();
        }
        #endregion

        #region DbSets
        public virtual DbSet<IdempotentRequest> IdempotentRequests { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Role> TenantRoles { get; set; }
        public virtual DbSet<Permission> TenantPermissions { get; set; }
        public virtual DbSet<RolePermission> TenantRolePermissions { get; set; }
        public virtual DbSet<TenantRoleUser> TenantRoleUsers { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<ItemTaxes> ItemTaxes { get; set; }
        public DbSet<PricingItem> PricingItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<FiscalSupport> FiscalSupports { get; set; }
        public DbSet<Deadline> Deadlines { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SpotPayment> SpotPayments { get; set; }
        public DbSet<SpotPayable> SpotPayables { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Receivable> Receivables { get; set; }
        public DbSet<Payable> Payables { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<PayableReceipt> PayableReceipts { get; set; }
        public DbSet<PettyCash> PettyCashes { get; set; }
        public DbSet<PettyCashMovement> PettyCashMovements { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentItem> AppointmentItems { get; set; }
        private void SetGlobalQuery<T>(ModelBuilder builder) where T : class, ITenant
        {
            builder.Entity<T>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        }
        #endregion
    }
}
