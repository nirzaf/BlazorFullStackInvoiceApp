using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazorInvoiceApp.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        DbSet<Invoice> invoices,
        DbSet<Customer> customers,
        DbSet<InvoiceTerms> invoiceTerms,
        DbSet<InvoiceLineItem> invoicesLineItems)
        : IdentityDbContext(options)
    {
        public DbSet<Invoice> Invoices { get; set; } = invoices;
        public DbSet<Customer> Customers { get; set; } = customers;
        public DbSet<InvoiceTerms> InvoiceTerms { get; set; } = invoiceTerms;
        public DbSet<InvoiceLineItem> InvoicesLineItems { get; set; } = invoicesLineItems;


        private void RemoveFixUps(ModelBuilder modelBuilder, Type type)
        {
            foreach (var relationship in modelBuilder.Model.FindEntityType(type)!.GetForeignKeys())
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // customizations
            RemoveFixUps(modelBuilder, typeof(Invoice));
            RemoveFixUps(modelBuilder, typeof(InvoiceTerms));
            RemoveFixUps(modelBuilder, typeof(Customer));
            RemoveFixUps(modelBuilder, typeof(InvoiceLineItem));

            modelBuilder.Entity<Invoice>().Property(u => u.InvoiceNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<InvoiceLineItem>()
               .Property(u => u.TotalPrice)
               .HasComputedColumnSql("[UnitPrice] * [Quantity]");

            base.OnModelCreating(modelBuilder);
        }
    }
}