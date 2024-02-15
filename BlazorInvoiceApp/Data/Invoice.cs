using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlazorInvoiceApp.Data;

public class Invoice : IEntity, IOwnedEntity
{
    public IdentityUser? User { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceNumber { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public string Description { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;
    public Customer? Customer { get; set; } = null!;

    public string InvoiceTermsId { get; set; } = string.Empty;
    public InvoiceTerms? InvoiceTerms { get; set; } = null!;

    public double Paid { get; set; } = 0;
    public double Credit { get; set; } = 0;

    public double TaxRate { get; set; } = 0;

    public ICollection<InvoiceLineItem> InvoiceLineItems { get; set; } = new List<InvoiceLineItem>();

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;
}