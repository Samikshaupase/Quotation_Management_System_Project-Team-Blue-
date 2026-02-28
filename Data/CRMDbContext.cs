using Microsoft.EntityFrameworkCore;
using QuotationManagementApp.Models;

namespace QuotationManagementApp.Data
{
    public class CRMDbContext : DbContext
    {
        public CRMDbContext(DbContextOptions<CRMDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationLineItem> QuotationLineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Quotation Configuration
            // =========================
            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.HasKey(q => q.QuotationId);

                entity.Property(q => q.QuoteNumber)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(q => q.Status)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(q => q.DiscountAmount)
                      .HasPrecision(18, 2);

                entity.Property(q => q.TotalAmount)
                      .HasPrecision(18, 2);

                entity.Property(q => q.QuoteDate)
                      .IsRequired();

                entity.Property(q => q.IsDeleted)
                      .HasDefaultValue(false);
            });

            // =========================
            // Line Item Configuration
            // =========================
            modelBuilder.Entity<QuotationLineItem>(entity =>
            {
                entity.HasKey(l => l.LineItemId);

                entity.Property(l => l.ProductName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(l => l.UnitPrice)
                      .HasPrecision(18, 2);

                entity.Property(l => l.LineTotal)
                      .HasPrecision(18, 2);

                entity.HasOne(l => l.Quotation)
                      .WithMany(q => q.LineItems)
                      .HasForeignKey(l => l.QuotationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}