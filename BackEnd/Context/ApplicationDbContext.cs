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

            modelBuilder.Entity<Warehouse>().Property(p => p.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Warehouse>().Property(p => p.Address).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Warehouse>().Property(p => p.Phone).HasMaxLength(10).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.Email).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.City).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.Country).HasMaxLength(2).IsRequired(false);
            modelBuilder.Entity<Warehouse>().Property(p => p.ZipCode).HasMaxLength(5).IsRequired(false);

            modelBuilder.Entity<Item>().Property(p => p.ItemType).HasConversion<int>().IsRequired();
            modelBuilder.Entity<Item>().Property(p => p.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Item>().Property(p => p.Reference).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<Item>().Property(p => p.Description).HasMaxLength(100).IsRequired(false);
            modelBuilder.Entity<Item>().Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Item>().Ignore(p => p.TotalPrice);

            //modelBuilder.Entity<PricingItem>().HasKey(g => new { g.ItemId, g.PricingId });
            modelBuilder.Entity<PricingItem>().Property(p => p.InitialCost).HasColumnType("decimal(18,2)").IsRequired();
            modelBuilder.Entity<PricingItem>().Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();

            modelBuilder.Entity<Item>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Item>()
                .HasOne(p => p.Unit)
                .WithMany(u => u.Items)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Item>()
                .HasOne(p => p.Brand)
                .WithMany(u => u.Items)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemTaxes>()
                .HasKey(g => new { g.TaxId, g.ItemId });

            modelBuilder.Entity<Stock>()
                .HasOne(ps => ps.Item)
                .WithMany(p => p.Stocks)
                .HasForeignKey(ps => ps.ItemId);

            modelBuilder.Entity<Stock>()
                .HasOne(ps => ps.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(ps => ps.WarehouseId);
        }
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
    }
}
