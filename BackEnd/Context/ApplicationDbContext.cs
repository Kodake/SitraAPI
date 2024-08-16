using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Category>().Property(p => p.Description).HasMaxLength(100).IsRequired(false);

            modelBuilder.Entity<Brand>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Brand>().Property(p => p.ImageURL).IsRequired(false);

            modelBuilder.Entity<Unit>().Property(p => p.Name).HasMaxLength(20);
            modelBuilder.Entity<Unit>().Property(p => p.ShortName).HasMaxLength(20);

            modelBuilder.Entity<Currency>().Property(p => p.Code).HasMaxLength(20);
            modelBuilder.Entity<Currency>().Property(p => p.Name).HasMaxLength(50);

            modelBuilder.Entity<Contact>().Property(p => p.ContactType).HasConversion<int>().IsRequired(false);
            modelBuilder.Entity<Contact>().Property(p => p.IdentificationType).HasConversion<int>().IsRequired(false);
            modelBuilder.Entity<Contact>().Property(p => p.Province).HasMaxLength(2).IsRequired(false);
            modelBuilder.Entity<Contact>().Property(p => p.Identification).HasMaxLength(15).IsRequired(false);
            modelBuilder.Entity<Contact>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Contact>().Property(p => p.Address).HasMaxLength(100);

            modelBuilder.Entity<Warehouse>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Warehouse>().Property(p => p.Address).HasMaxLength(100);
            modelBuilder.Entity<Warehouse>().Property(p => p.Phone).HasMaxLength(10).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.Email).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.City).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.Country).HasMaxLength(2).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.ZipCode).HasMaxLength(5).IsRequired(false);

            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Reference).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(100).IsRequired(false);
            modelBuilder.Entity<Product>().Property(p => p.InitialCost).HasColumnType("decimal(18,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Ignore(p => p.TotalPrice);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTaxes>()
                .HasKey(g => new { g.TaxId, g.ProductId });

            modelBuilder.Entity<Stock>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductStocks)
                .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<Stock>()
                .HasOne(ps => ps.Warehouse)
                .WithMany(w => w.ProductStocks)
                .HasForeignKey(ps => ps.WarehouseId);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<ProductTaxes> ProductTaxes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
    }
}
