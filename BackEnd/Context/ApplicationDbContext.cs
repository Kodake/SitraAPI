using BackEnd.Context.Accounts;
using BackEnd.Context.AdministrativeTools;
using BackEnd.Context.Auths;
using BackEnd.Context.CashBoxes;
using BackEnd.Context.Contacts;
using BackEnd.Context.Finances;
using BackEnd.Context.Invoices;
using BackEnd.Context.Products;
using BackEnd.Context.Warehouses;
using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Auths;
using BackEnd.Entities.CashBoxes;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Invoices;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PayableConfiguration());
            modelBuilder.ApplyConfiguration(new PayableReceiptConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentReceiptConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivableConfiguration());

            modelBuilder.ApplyConfiguration(new DeadlineConfiguration());
            modelBuilder.ApplyConfiguration(new FiscalSupportConfiguration());

            modelBuilder.ApplyConfiguration(new ContactConfiguration());

            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTaxesConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());

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
            modelBuilder.ApplyConfiguration(new PricingConfiguration());
            modelBuilder.ApplyConfiguration(new PricingItemConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());

            modelBuilder.ApplyConfiguration(new StockConfiguration());
            modelBuilder.ApplyConfiguration(new TransferConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());

            modelBuilder.ApplyConfiguration(new PettyCashConfiguration());
            modelBuilder.ApplyConfiguration(new PettyCashMovementConfiguration());

            modelBuilder.ApplyConfiguration(new AuthConfiguration()).HasDefaultSchema("identity");
        }
        #endregion

        #region DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tax> Taxes { get; set; }
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
        #endregion
    }
}
