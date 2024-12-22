using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;
using Microsoft.EntityFrameworkCore;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Invoices;
using BackEnd.Entities.Accounts;
using BackEnd.Context.Finances;
using BackEnd.Context.Accounts;
using BackEnd.Context.Warehouses;
using BackEnd.Context.Products;
using BackEnd.Context.Contacts;
using BackEnd.Context.Invoices;
using BackEnd.Context.AdministrativeTools;

namespace BackEnd.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PaymentReceiptConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivableConfiguration());

            modelBuilder.ApplyConfiguration(new DeadlineConfiguration());
            modelBuilder.ApplyConfiguration(new FiscalSupportConfiguration());

            modelBuilder.ApplyConfiguration(new ContactConfiguration());

            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTaxesConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());

            modelBuilder.ApplyConfiguration(new SaleInvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new SaleInvoiceItemConfiguration());
            modelBuilder.ApplyConfiguration(new SpotPaymentConfiguration());

            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new PricingConfiguration());
            modelBuilder.ApplyConfiguration(new PricingItemConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());

            modelBuilder.ApplyConfiguration(new StockConfiguration());
            modelBuilder.ApplyConfiguration(new TransferConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
        }
        #endregion

        #region DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
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
        public DbSet<SaleInvoice> SaleInvoices { get; set; }
        public DbSet<SaleInvoiceItem> SaleInvoiceItems { get; set; }
        public DbSet<Receivable> Receivables { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        #endregion
    }
}
